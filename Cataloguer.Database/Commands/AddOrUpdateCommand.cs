using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Cataloguer.Database.Models;

namespace Cataloguer.Database.Commands
{
    public class AddOrUpdateCommand : AbstractCommand
    {
        public AddOrUpdateCommand(DataBaseConfiguration config) : base(config) { }

        /// <param name="brochureId">Идентификатор каталога</param>
        /// <param name="goodsInBrochure">Список идентификаторов товаров с соответствующими ценами на них</param>
        public void AddPositions(int brochureId, IEnumerable<(int GoodId, decimal Price)> goodsInBrochure)
        {
            Context.BrochurePositions
                .AddRange(goodsInBrochure
                    .Select(x => new BrochurePosition()
                    {
                        BrochureId = brochureId,
                        Price = x.Price,
                        GoodId = x.GoodId,
                    }));

            Context.SaveChanges();
        }

        /// <summary>
        /// Возвращает id созданной/обновленной сущности
        /// </summary>
        public int AddOrUpdate(Brochure brochure)
        {
            Brochure? entity = Context.Brochures.FirstOrDefault(x => x.Id == brochure.Id)
                ?? Context.Brochures.Add(brochure).Entity;

            entity.StatusId = brochure.StatusId;
            entity.Name = brochure.Name;
            entity.Date = brochure.Date;
            entity.Edition = brochure.Edition;
            entity.PositionCount = Context.BrochurePositions
                .Where(x => x.BrochureId == entity.Id)
                .Count();

            Context.Update(entity);
            Context.SaveChanges();

            return entity.Id;
        }

        /// <summary>
        /// Возвращает id созданной/обновленной сущности
        /// </summary>
        public int AddOrUpdate(Distribution distribution)
        {
            var entity = Context.Distributions.FirstOrDefault(x => x.Id == distribution.Id)
                ?? Context.Distributions.Add(distribution).Entity;

            entity.BrochureId = distribution.BrochureId;
            entity.AgeGroupId = distribution.AgeGroupId;
            entity.GenderId = distribution.GenderId;
            entity.BrochureCount = distribution.BrochureCount;
            entity.TownId = distribution.TownId;

            Context.Update(entity);
            Context.SaveChanges();

            return entity.Id;
        }
    }
}
