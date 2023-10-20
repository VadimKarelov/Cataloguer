using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Cataloguer.Database.Models;

namespace Cataloguer.Database.Commands.AddOrUpdateCommand
{
    public class AddOrUpdateBrochureCommand : AbstractCommand
    {
        public AddOrUpdateBrochureCommand(DataBaseConfiguration config) : base(config) { }

        public void AddOrUpdate(Brochure brochure)
        {
            Brochure? entity = Context.Brochures.FirstOrDefault(x => x.Id == brochure.Id)
                ?? Context.Brochures.Add(brochure).Entity;

            entity.StatusId = brochure.StatusId;
            entity.Name = brochure.Name;
            entity.Date = brochure.Date;
            entity.Edition = Context.Distributions.Sum(x => x.BrochureCount);
            entity.PositionCount = Context.BrochurePositions
                .Where(x => x.BrochureId == entity.Id)
                .Count();

            Context.Update(entity);
            Context.SaveChanges();
        }
    }
}
