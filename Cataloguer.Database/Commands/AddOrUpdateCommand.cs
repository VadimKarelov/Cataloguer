using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Cataloguer.Database.Models;
using Cataloguer.Database.Models.SpecialModels.InputApiModels;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Cataloguer.Database.Commands
{
    public class AddOrUpdateCommand : AbstractCommand
    {
        public AddOrUpdateCommand(DataBaseConfiguration config) : base(config) { }

        /// <param name="brochureId">Идентификатор каталога</param>
        /// <param name="goodsInBrochure">Список идентификаторов товаров с соответствующими ценами на них</param>
        public void AddPositions(int brochureId, IEnumerable<CreationPosition> goodsInBrochure)
        {
            Log.Information($"Вызван запрос на добавление товаров в каталог с id={brochureId}. Количество товаров {goodsInBrochure.Count()}.");

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
        }

        /// <summary>
        /// Возвращает id созданной/обновленной сущности
        /// </summary>
        public int AddOrUpdate(Brochure brochure)
        {
            Log.Information($"Вызван запрос на добавление или обновление каталога.");

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

            return entity.Id;
        }

        /// <summary>
        /// Возвращает id созданной/обновленной сущности
        /// </summary>
        public int AddOrUpdate(Distribution distribution)
        {
            Log.Information($"Вызван запрос на добавление или обновление рассылки.");

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

            return entity.Id;
        }
    }
}
