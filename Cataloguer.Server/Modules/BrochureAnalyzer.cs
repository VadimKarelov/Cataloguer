using Accord.Statistics.Models.Regression.Linear;
using Cataloguer.Common.Models;
using Cataloguer.Common.Models.SpecialModels.OutputApiModels;
using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.AddOrUpdateCommands;
using Cataloguer.Database.Commands.GetCommands;
using Serilog;
using System.Security.Cryptography.Xml;
using System.Windows.Markup;

namespace Cataloguer.Server.Modules;

public static class BrochureAnalyzer
{
    public static string TryComputeBrochureIncome(DataBaseConfiguration config, int brochureId)
    {
        try
        {
            var res = ComputeBrochurePotentialIncome(config, brochureId);

            // обновляем данные в таблице
            var brochure = new GetCommand(config).GetBrochure(brochureId);
            brochure.PotentialIncome = res;
            new AddOrUpdateCommand(config).AddOrUpdate(brochure);

            return Math.Round(res, 2).ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Произошла ошибка во время вычисления эффективности каталога.");
            return ex.Message;
        }
    }

    private static decimal ComputeBrochurePotentialIncome(DataBaseConfiguration config, int brochureId)
    {
        var brochure = new GetCommand(config).GetBrochure(brochureId, true);

        if (brochure == null)
            throw new Exception($"Каталог с id={brochureId} не найден в базе данных!");

        var distributions = new GetCommand(config).GetListDistribution(x => x.BrochureId == brochureId, true);

        if (distributions == null)
            throw new Exception($"Каталог с id={brochureId} не содержит рассылок!");

        var goodsFromBrochure = new GetSpecialRequestCommand(config).GetGoodsFromBrochure(brochureId);

        if (goodsFromBrochure == null)
            throw new Exception($"Каталог с id={brochureId} не содержит товаров!");

        decimal income = 0;

        foreach (var distrib in distributions)
        {
            // список товаров из истории, которые есть в каталоге и в истории с фильтром по конкретной рассылке
            var historyForDistribution = new GetSpecialRequestCommand(config).GetGoodsForBrochureDistribution(brochureId, distrib.Id);

            // список только товаров, не повторяющихся
            var goods = historyForDistribution.Select(x => x.Good).Distinct().ToList();

            // идем по товарам
            foreach (var good in goods)
            {
                // записи из истории по конкретному товару
                var historyNotes = historyForDistribution.Where(x => x.GoodId == good!.Id);

                // количество проданных товаров по рассылке
                var n = historyNotes.Count();

                income += n * historyNotes.Average(x => x.Price);
            }
        }

        return income;
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

    public static List<SellHistoryForChart> GetPredictedSellHistoryForChart(DataBaseConfiguration config, int brochureId)
    {
        var brochure = new GetCommand(config).GetBrochure(brochureId);

        if (brochure == null) return new List<SellHistoryForChart>();

        var distributions = new GetCommand(config).GetListDistribution(x => x.BrochureId == brochureId);

        // история продаж по определенной рассылке с товарами из каталога (отсортировано по дате)
        var sellHistory = new GetSpecialRequestCommand(config).GetSellHistoryForBrochureGoodsAndDistributions(brochureId);

        if (!sellHistory.Any())
        {
            Log.Error($"Для каталога id={brochureId} не нашлось истории покупок!");
            SetNewPotentialIncomeToBrochure(config, brochure, 0);
            return new List<SellHistoryForChart>();
        }

        DateTime minDate = DateTime.MaxValue, maxDate = DateTime.MinValue;
        foreach (var sell in sellHistory)
        {
            if (sell.SellDate > maxDate)
                maxDate = sell.SellDate;
            if (sell.SellDate < minDate)
                minDate = sell.SellDate;
        }

        var values = new List<(double x, double y)>();
        
        for (DateTime date = minDate; date <= maxDate; date.AddDays(1))
        {
            var sales = sellHistory.Where(x => x.SellDate.Date == date).Select(x => x.Price);

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
        for (DateTime date = brochure.Date; date <= date.AddDays(30); date.AddDays(1))
        {
            prediction.Add(new SellHistoryForChart()
            {
                Date = date,
                Income = (decimal)regression.Transform(date.ToOADate())
            });
        }

        SetNewPotentialIncomeToBrochure(config, brochure, prediction.Sum(x => x.Income));

        return prediction;
    }

    private static void SetNewPotentialIncomeToBrochure(DataBaseConfiguration config, Brochure brochure, decimal income)
    {
        brochure.PotentialIncome = income;
        new AddOrUpdateCommand(config).AddOrUpdate(brochure);
    }
}