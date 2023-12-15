using Accord.Statistics.Models.Regression.Linear;
using Cataloguer.Common.Models.SpecialModels.OutputApiModels;
using Cataloguer.Database.Base;
using Cataloguer.Database.Commands;
using Cataloguer.Database.Commands.AddOrUpdateCommands;
using Cataloguer.Database.Commands.GetCommands;
using Serilog;

namespace Cataloguer.Server.Modules;

public static class BrochureAnalyzer
{
    public static string TryComputeBrochureIncome(DataBaseConfiguration config, int brochureId)
    {
        try
        {
            TryComputeBrochurePotentialIncome(config, brochureId);

            // обновляем данные в таблице
            var brochure = new GetCommand(config).GetBrochure(brochureId);

            return Math.Round(brochure.PotentialIncome, 2).ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Произошла ошибка во время вычисления эффективности каталога.\n{ex.Message}");
            return ex.Message;
        }
    }

    /*
    // private static void TryComputeBrochurePotentialIncome(DataBaseConfiguration config, int brochureId)
    // {
    //     var brochure = new GetCommand(config).GetBrochure(brochureId);
    //     if (brochure == null)
    //         throw new Exception($"Каталог с id={brochureId} не найден в базе данных!");
    //     
    //     // история продаж по определенной рассылке с товарами из каталога (отсортировано по дате)
    //     var sellHistory = new GetSpecialRequestCommand(config).GetSellHistoryForBrochureGoodsAndDistributions(brochureId);
    //
    //     if (!sellHistory.Any())
    //         throw new Exception($"Для каталога id={brochureId} недостаточно данных для расчета эффективности! Добавьте больше товаров или рассылок");
    //     
    //     // начало расчета
    //     
    //     decimal income = 0;
    //     
    //     DateTime minDate = DateTime.MaxValue, maxDate = DateTime.MinValue;
    //     foreach (var sell in sellHistory)
    //     {
    //         if (sell.SellDate > maxDate)
    //             maxDate = sell.SellDate;
    //         if (sell.SellDate < minDate)
    //             minDate = sell.SellDate;
    //     }
    //
    //     var values = new List<(double x, double y)>();
    //     
    //     for (DateTime date = minDate; date <= maxDate; date = date.AddDays(1))
    //     {
    //         var sales = sellHistory
    //             .Where(x => x.SellDate.Date.Date == date.Date)
    //             .Select(x => x.Price);
    //
    //         // в какие-то дни могло не быть продаж
    //         double sum = sales.Any() ? (double)sales.Sum() : 0;
    //
    //         values.Add((date.ToOADate(), sum));
    //     }
    //
    //     // подготовленные для обучения данные
    //     var xs = values.Select(x => x.x).ToArray();
    //     var ys = values.Select(x => x.y).ToArray();
    //
    //     // обучение
    //     SimpleLinearRegression regression = new OrdinaryLeastSquares().Learn(xs, ys);
    //
    //     List<SellHistoryForChart> prediction = new();
    //
    //     // предсказание
    //     for (DateTime date = brochure.Date; date <= brochure.Date.AddDays(30); date = date.AddDays(1))
    //     {
    //         prediction.Add(new SellHistoryForChart()
    //         {
    //             Date = date,
    //             Income = Math.Max((decimal)regression.Transform(date.ToOADate()), 0)
    //         });
    //     }
    //
    //     // удаление старой истории
    //     new DeleteCommand(config).DeletePredictionHistory(brochureId);
    //     
    //     // сохранение данных
    //     new SpecialAddOrUpdateCommand(config).AddPredictedHistory(brochureId, prediction);
    // }
    */
    
    public static void TryComputeBrochurePotentialIncome(DataBaseConfiguration config, int brochureId)
    {
        const short regressionPow = 3;
        
        var brochure = new GetCommand(config).GetBrochure(brochureId);
        if (brochure == null)
            throw new Exception($"Каталог с id={brochureId} не найден в базе данных!");

        new SpecialAddOrUpdateCommand(config).GenerateSellHistoryIfNotExist(brochureId);
        
        var distributions = new GetCommand(config).GetListDistribution(x => x.BrochureId == brochureId);

        bool enoughData = false;

        var predictionMinDate = DateTime.Today < brochure.Date
            ? DateTime.Today.AddDays(-30).Date
            : brochure.Date.AddDays(-30).Date;

        var predictionMaxDate = brochure.Date.AddDays(30);
        
        // результат по всем рассылкам (key=date, value=income)
        var prediction = new Dictionary<double, double>();

        foreach (var distribution in distributions)
        {
            // история продаж по определенной рассылке с товарами из каталога (отсортировано по дате)
            var sellHistory = new GetSpecialRequestCommand(config).GetGoodsForBrochureDistribution(brochureId, distribution.Id);
            
            if (!sellHistory.Any()) continue;
            enoughData = true; // смотрим, что хоть где-то хватило данных
            
            DateTime minDate = DateTime.MaxValue, maxDate = DateTime.MinValue;
            foreach (var sell in sellHistory)
            {
                if (sell.SellDate > maxDate)
                    maxDate = sell.SellDate;
                if (sell.SellDate < minDate)
                    minDate = sell.SellDate;
            }

            // Готовим обучающие данные
            var values = new List<(double x, double y)>();

            for (DateTime date = minDate; date <= maxDate; date = date.AddDays(1))
            {
                var sales = sellHistory
                    .Where(x => x.SellDate.Date.Date == date.Date)
                    .Select(x => x.Price);

                // в какие-то дни могло не быть продаж
                double sum = sales.Any() ? (double)sales.Sum() : 0;

                values.Add((date.ToOADate(), sum));
            }

            var xs = values.Select(x => x.x).ToArray();
            var ys = values.Select(x => x.y).ToArray();

            // обучение
            SimpleLinearRegression regression = new OrdinaryLeastSquares().Learn(xs, ys);
            //MultivariateLinearRegression regression = new OrdinaryLeastSquares().Learn(xs, ys, regressionPow);

            // предсказание для рассылки
            for (DateTime date = predictionMinDate; date <= predictionMaxDate; date = date.AddDays(1))
            {
                //var inc = Math.Max(regression.Transform(date.ToOADate(), regressionPow), 0);
                var inc = regression.Transform(date.ToOADate());
                AddToDictionary(prediction, date.ToOADate(), inc);
            }
        }
        
        if (!enoughData)
            throw new Exception($"Для каталога id={brochureId} недостаточно данных для расчета эффективности! Добавьте больше товаров или рассылок");
        
        List<SellHistoryForChart> predictionChart = new();

        // предсказание
        for (DateTime date = predictionMinDate; date <= predictionMaxDate; date = date.AddDays(1))
        {
            predictionChart.Add(new SellHistoryForChart()
            {
                Date = date,
                Income = (decimal)prediction[date.ToOADate()]
            });
        }

        // удаление старой истории
        new DeleteCommand(config).DeletePredictionHistory(brochureId);
        
        // сохранение данных
        new SpecialAddOrUpdateCommand(config).AddPredictedHistory(brochureId, predictionChart);
        
    }

    private static void AddToDictionary(Dictionary<double, double> dictionary, double key, double value)
    {
        if (!dictionary.TryAdd(key, value))
        {
            dictionary[key] += value;
        }
    }
}