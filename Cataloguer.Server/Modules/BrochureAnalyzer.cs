using Cataloguer.Database.Base;
using Cataloguer.Database.Commands;
using Cataloguer.Database.Commands.GetCommands;
using Cataloguer.Database.Models;
using Serilog;

namespace Cataloguer.Server.Modules
{
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
                    .GetGoodsFromSellHistory(brochure);

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
                        x.Age >= distrib.AgeGroup.MinimalAge && x.Age <= distrib.AgeGroup.MaximalAge)
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

                brochure.Status = distributionsEfficiency.Average() >= 0.5 ?
                    statuses.FirstOrDefault(x => x.Name == "Эффективный") :
                    statuses.FirstOrDefault(x => x.Name == "Не эффективный");

                new AddOrUpdateCommand(config).AddOrUpdate(brochure);

                return brochure.StatusId;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Произошла ошибка во время вычисления эффективности каталога.");
                return -1;
            }
        }
    }
}
