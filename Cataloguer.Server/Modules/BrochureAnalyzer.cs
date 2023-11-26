using Cataloguer.Common.Models;
using Cataloguer.Database.Base;
using Cataloguer.Database.Commands;
using Cataloguer.Database.Commands.GetCommands;
using Serilog;

namespace Cataloguer.Server.Modules;

public static class BrochureAnalyzer
{
    /// <returns>Возвращает идентификатор статуса проверки. Вернет -1, если произошла ошибка.</returns>
    public static int ComputeBrochureEfficiency(DataBaseConfiguration config, Brochure brochure)
    {
        try
        {
            /* Общий принцип: проверка наличия записей по каждой рассылке в истории.
             * Для каждой рассылки смотрим: продавался ли товар в этом городе,
             * продавался ли конкретной возрастной группе,
             * продавался ли конкретному полу.
             * В конце суммируем все показатели по рассылке и вычисляем среднее.
             * Находим среднее арифметическое по всем рассылкам. И присваиваем статус.
             */

            // история продажи товаров из конкретного каталога
            var sellHistory = new GetSpecialRequestCommand(config)
                .GetGoodsFromSellHistory(brochure.Id);

            var distributions = new GetCommand(config)
                .GetListDistribution(x => x.BrochureId == brochure.Id);

            var goodsInBrochure = sellHistory.Select(x => x.Good)
                .Distinct();

            List<double> distributionsEfficiency = new();

            foreach (var distrib in distributions)
            {
                // получаем товары по конкретному городу, age группе и полу
                var goodsInDistrib = sellHistory.Where(x => x.TownId == distrib.TownId &&
                                                            x.GenderId == distrib.GenderId &&
                                                            x.Age >= distrib.AgeGroup.MinimalAge &&
                                                            x.Age <= distrib.AgeGroup.MaximalAge)
                    .Select(x => x.Good)
                    .Distinct();

                // вычисляем - сколько товаров из каталога было продано в городе,
                // конкретному полу и возрастной группе
                var soldGoodsNumber = goodsInBrochure.Intersect(goodsInDistrib)
                    .Count();

                // оцениваем: сколько товаров из каталога продавались в городе (доля товаров каталога)
                distributionsEfficiency.Add((double)soldGoodsNumber / goodsInBrochure.Count());
            }

            var statuses = new GetCommand(config).GetListStatus();

            brochure.Status = distributionsEfficiency.Average() >= 0.5
                ? statuses.FirstOrDefault(x => x.Name == "Эффективный")
                : statuses.FirstOrDefault(x => x.Name == "Не эффективный");

            new AddOrUpdateCommand(config).AddOrUpdate(brochure);

            return brochure.StatusId;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Произошла ошибка во время вычисления эффективности каталога.");
            return -1;
        }
    }

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
}