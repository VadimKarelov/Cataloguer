using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Cataloguer.Database.Models;
using Cataloguer.Database.Models.SpecialModels.InputApiModels;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Cataloguer.Database.Commands
{
    public class AddOrUpdateCommand : AbstractCommand
    {
        public AddOrUpdateCommand(DataBaseConfiguration config) : base(config) { }

        /// <param name="brochureId">Идентификатор каталога</param>
        /// <param name="goodsInBrochure">Список идентификаторов товаров с соответствующими ценами на них</param>
        [MethodName("добавление позиций в каталог")]
        public void AddPositions(int brochureId, IEnumerable<CreationPosition> goodsInBrochure)
        {
            StartExecuteCommand(MethodBase.GetCurrentMethod(), brochureId, goodsInBrochure);

            var brochure = Context.Brochures
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == brochureId);

            if (brochure == null)
                return;

            Context.BrochurePositions
                .AddRange(goodsInBrochure
                    .Select(x => new BrochurePosition()
                    {
                        BrochureId = brochureId,
                        Price = x.Price,
                        GoodId = x.GoodId,
                    }));

            brochure.PositionCount = Context.BrochurePositions
                .AsNoTracking()
                .Where(x => x.BrochureId == brochureId)
                .Count();

            Context.SaveChanges();

            FinishExecuteCommand(MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Возвращает id созданной/обновленной сущности
        /// </summary>
        [MethodName("Добавление/обновление каталога")]
        public int AddOrUpdate(Brochure brochure)
        {
            StartExecuteCommand(MethodBase.GetCurrentMethod(), brochure);

            Brochure? entity = Context.Brochures.FirstOrDefault(x => x.Id == brochure.Id);

            bool isNew = false;

            if (entity == null)
            {
                isNew = true;
                entity = new();
            }

            entity.StatusId = brochure.StatusId;
            entity.Name = brochure.Name;
            entity.Date = brochure.Date;
            entity.Edition = brochure.Edition;
            entity.PositionCount = Context.BrochurePositions
                .Where(x => x.BrochureId == entity.Id)
                .Count();

            if (Context.Statuses.FirstOrDefault(x => x.Id == entity.StatusId) == null)
            {
                entity.StatusId = Context.Statuses.FirstOrDefault(x => x.Name == "Не проверено").Id;
            }

            if (isNew)
            {
                Context.Add(entity);
            }
            else
            {
                Context.Update(entity);
            }

            Context.SaveChanges();

            FinishExecuteCommand(MethodBase.GetCurrentMethod(), entity.Id);

            return entity.Id;
        }

        /// <summary>
        /// Возвращает id созданной/обновленной сущности
        /// </summary>
        [MethodName("Добавление/обновление рассылки")]
        public int AddOrUpdate(Distribution distribution)
        {
            StartExecuteCommand(MethodBase.GetCurrentMethod(), distribution);

            var entity = Context.Distributions.FirstOrDefault(x => x.Id == distribution.Id);

            bool isNew = false;

            if (entity == null)
            {
                isNew = true;
                entity = new();
            }

            entity.BrochureId = distribution.BrochureId;
            entity.AgeGroupId = distribution.AgeGroupId;
            entity.GenderId = distribution.GenderId;
            entity.BrochureCount = distribution.BrochureCount;
            entity.TownId = distribution.TownId;

            // проверка, что каталог существует
            var brochure = Context.Brochures.FirstOrDefault(x => x.Id == entity.BrochureId);

            if (brochure == null)
                return -1;

            // Подсчет тиража у текущего каталога
            var distributionsCount = Context.Distributions
                .AsNoTracking()
                .Where(x => x.BrochureId == brochure.Id)
                .Sum(x => x.BrochureCount) + entity.BrochureCount;

            // Проверка, чтобы нельзя было создать тираж рассылки больше, чем задано в каталоге
            if (distributionsCount > brochure.Edition)
                return -1;

            if (isNew)
            {
                Context.Add(entity);
            }
            else
            {
                Context.Update(entity);
            }

            Context.SaveChanges();

            StartExecuteCommand(MethodBase.GetCurrentMethod(), entity.Id);

            return entity.Id;
        }
    }
}
