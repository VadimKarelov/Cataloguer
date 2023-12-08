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

    private static void TryComputeBrochurePotentialIncome(DataBaseConfiguration config, int brochureId)
    {
        var brochure = new GetCommand(config).GetBrochure(brochureId);
        if (brochure == null)
            throw new Exception($"Каталог с id={brochureId} не найден в базе данных!");
        
        // история продаж по определенной рассылке с товарами из каталога (отсортировано по дате)
        var sellHistory = new GetSpecialRequestCommand(config).GetSellHistoryForBrochureGoodsAndDistributions(brochureId);

        if (!sellHistory.Any())
            throw new Exception($"Для каталога id={brochureId} недостаточно данных для расчета эффективности! Добавьте больше товаров или рассылок");
        
        // начало расчета
        
        decimal income = 0;
        
        DateTime minDate = DateTime.MaxValue, maxDate = DateTime.MinValue;
        foreach (var sell in sellHistory)
        {
            if (sell.SellDate > maxDate)
                maxDate = sell.SellDate;
            if (sell.SellDate < minDate)
                minDate = sell.SellDate;
        }

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

        // подготовленные для обучения данные
        var xs = values.Select(x => x.x).ToArray();
        var ys = values.Select(x => x.y).ToArray();

        // обучение
        SimpleLinearRegression regression = new OrdinaryLeastSquares().Learn(xs, ys);

        List<SellHistoryForChart> prediction = new();

        // предсказание
        for (DateTime date = brochure.Date; date <= brochure.Date.AddDays(30); date = date.AddDays(1))
        {
            prediction.Add(new SellHistoryForChart()
            {
                Date = date,
                Income = (decimal)regression.Transform(date.ToOADate())
            });
        }

        // удаление старой истории
        new DeleteCommand(config).DeletePredictionHistory(brochureId);
        
        // сохранение данных
        new SpecialAddOrUpdateCommand(config).AddPredictedHistory(brochureId, prediction);
    }


    /*
     * незавершенный вариант с подсчетом статистики для каждой рассылки отдельно (если будем внедрять что-то сложнее линейной регрессии)
    public static List<SellHistoryForChart> GetPredictedSellHistoryForChart(DataBaseConfiguration config, int brochureId)
    {
        var brochure = new GetCommand(config).GetBrochure(brochureId);

        if (brochure == null) return new List<SellHistoryForChart>();

        var distributions = new GetCommand(config).GetListDistribution(x => x.BrochureId == brochureId);

        foreach (var distribution in distributions)
        {
            // история продаж по определенной рассылке с товарами из каталога (отсортировано по дате)
            var sellHistory = new GetSpecialRequestCommand(config).GetGoodsForBrochureDistribution(brochureId, distribution.Id);

            DateTime minDate = DateTime.MaxValue, maxDate = DateTime.MinValue;
            foreach (var sell in sellHistory)
            {
                if (sell.SellDate > maxDate)
                    maxDate = sell.SellDate;
                if (sell.SellDate < minDate)
                    minDate = sell.SellDate;
            }

            // Готовим обучающие данные
            var values = new List<(double x, double y)> ();

            for (double date = minDate.ToOADate(); date <= maxDate.ToOADate(); date += 1)
            {
                var sales = sellHistory.Where(x => x.SellDate.Date == DateTime.FromOADate(date)).Select(x => x.Price);

                // в какие-то дни могло не быть продаж
                var sum = sales.Any() ? (double)sales.Sum() : 0;

                values.Add((date, sum));
            }

            var xs = values.Select(x => x.x).ToArray();
            var ys = values.Select(x => x.y).ToArray();


        }
    }*/
}