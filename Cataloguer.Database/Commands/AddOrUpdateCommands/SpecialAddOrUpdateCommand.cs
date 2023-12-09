using Cataloguer.Common.Models;
using Cataloguer.Common.Models.SpecialModels.OutputApiModels;
using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Cataloguer.Database.Commands.GetCommands;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Commands.AddOrUpdateCommands
{
    public class SpecialAddOrUpdateCommand : AbstractCommand
    {
        public SpecialAddOrUpdateCommand(DataBaseConfiguration config) : base(config)
        {
        }

        /// <summary>
        /// Вернет ОК или ошибку
        /// </summary>
        public string TryMarkBrochureAsReleased(int brochureId)
        {
            var brochure = Context.Brochures
                .Include(x => x.Status)
                .FirstOrDefault(x => x.Id == brochureId);

            RememberState(brochure);

            if (brochure == null)
            {
                return $"Каталога с Id={brochureId} не существует!";
            }

            if (brochure.Status!.Name == Status.Released.Name)
            {
                return $"OK";
            }

            if (brochure.PotentialIncome == 0)
            {
                return $"Доход каталога с Id={brochureId} не был рассчитан!";
            }

            brochure.Status = Context.Statuses
                .FirstOrDefault(x => x.Name == Status.Released.Name);

            Context.Update(brochure);
            Context.SaveChanges();

            LogChange(brochure);
            return $"OK";
        }

        /// <summary>
        /// Добавит прогноз и посчитает выручку каталога
        /// </summary>
        public void AddPredictedHistory(int brochureId, List<SellHistoryForChart> prediction)
        {
            var brochure = Context.Brochures.FirstOrDefault(x => x.Id == brochureId);

            if (brochure == null) return;

            // расчет выручки каталога
            RememberState(brochure);
            brochure.PotentialIncome = prediction.Sum(x => x.Income);
            Context.Update(brochure);

            // сохранение истории
            var entitiesToAdd = prediction
                .Select(x => new PredictedSellHistory()
                {
                    BrochureId = brochureId,
                    PredictionDate = new DateOnly(x.Date.Year, x.Date.Month, x.Date.Day),
                    Value = x.Income,
                }).ToList();

            Context.PredictedSellHistory.AddRange(entitiesToAdd);
            Context.SaveChanges();

            foreach (var entity in entitiesToAdd)
                LogChange(null, entity);
        }

        /// <summary>
        /// Muhleg 2.0
        /// </summary>
        public void GenerateSellHistoryIfNotExist(int brochureId)
        {
            var sellHistory = new GetSpecialRequestCommand(DBConfig)
                .GetSellHistoryForBrochureGoodsAndDistributions(brochureId);
            
            if (sellHistory.Count() >= 100) return;
            
            GenerateSellHistory(brochureId);
        }
        
        /// <summary>
        /// Muhleg 2.0
        /// </summary>
        public void GenerateSellHistory(int brochureId)
        {
            var distribs = Context.Distributions
                .AsNoTracking()
                .Where(x => x.BrochureId == brochureId)
                .Include(x => x.AgeGroup)
                .ToList();

            if (!distribs.Any()) return;

            var goods = Context.BrochurePositions
                .AsNoTracking()
                .Where(x => x.BrochureId == brochureId)
                .ToList();

            if (!goods.Any()) return;

            // число месяца, когда должен быть пик покупок
            const int pickDate = 15;

            // число месяцев, в которые нужно сгенерировать историю
            const short numberOfMonths = 1;

            DateTime initDate = DateTime.Today.Date.AddDays(-30 * numberOfMonths);
            DateTime finishDate = DateTime.Today.Date;

            Random random = new Random();

            for (var date = initDate; date <= finishDate; date = date.AddDays(1))
            {
                var numberOfBuys = 5 + 8.0 / RangeToDate(date.Day, pickDate);
                for (int i = 0; i < numberOfBuys; i++)
                {
                    var good = goods[random.Next(goods.Count)];
                    var distr = distribs[random.Next(distribs.Count)];

                    var entity = new SellHistory()
                    {
                        GoodId = good.GoodId,
                        TownId = distr.TownId,
                        Age = (short)random.Next(distr.AgeGroup.MinimalAge, distr.AgeGroup.MaximalAge),
                        GenderId = distr.GenderId,
                        SellDate = date,
                        GoodCount = random.Next(1, 5),
                        Price = Context.SellHistory
                            .Where(x => x.GoodId == good.GoodId)
                            .Select(x => x.Price)
                            .AsEnumerable()
                            .DefaultIfEmpty(random.Next(500, 1000))
                            .Average() + random.Next(-100, 100)
                    };

                    Context.SellHistory.Add(entity);
                }
            }

            Context.SaveChanges();
        }

        private int RangeToDate(int currentDay, int targetDay)
        {
            // 1 не убирать, чтобы не было бесконечности при делении на ноль
            return Math.Min(Math.Abs(targetDay - currentDay), Math.Abs(targetDay - (currentDay - 31))) + 1;
        }
    }
}
