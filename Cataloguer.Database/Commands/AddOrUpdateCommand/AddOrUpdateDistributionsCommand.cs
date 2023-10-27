using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Cataloguer.Database.Models;

namespace Cataloguer.Database.Commands.AddOrUpdateCommand
{
    public class AddOrUpdateDistributionsCommand : AbstractCommand
    {
        public AddOrUpdateDistributionsCommand(DataBaseConfiguration config) : base(config) { }

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
