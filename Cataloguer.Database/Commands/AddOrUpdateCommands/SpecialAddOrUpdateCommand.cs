using Cataloguer.Common.Models;
using Cataloguer.Common.Models.SpecialModels.OutputApiModels;
using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
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
    }
}
