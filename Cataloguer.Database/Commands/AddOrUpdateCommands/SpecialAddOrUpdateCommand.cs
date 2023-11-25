using Cataloguer.Common.Models;
using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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
            StartExecuteCommand(MethodBase.GetCurrentMethod(), brochureId);

            var brochure = Context.Brochures
                .Include(x => x.Status)
                .FirstOrDefault(x => x.Id == brochureId);

            if (brochure == null)
            {
                FinishExecuteCommand(MethodBase.GetCurrentMethod(), "Несуществующий каталог");
                return $"Каталога с Id={brochureId} не существует!";                
            }

            if (brochure.Status!.Name == Status.Released.Name)
            {
                FinishExecuteCommand(MethodBase.GetCurrentMethod(), "Каталог уже выпущен");
                return $"OK";
            }

            if (brochure.PotentialIncome == 0)
            {
                FinishExecuteCommand(MethodBase.GetCurrentMethod(), "Нет расчета");
                return $"Доход каталога с Id={brochureId} не был рассчитан!";
            }

            brochure.Status = Context.Statuses
                .FirstOrDefault(x => x.Name == Status.Released.Name);

            Context.Update(brochure);
            Context.SaveChanges();

            FinishExecuteCommand(MethodBase.GetCurrentMethod(), "OK");
            return $"OK";
        }
    }
}
