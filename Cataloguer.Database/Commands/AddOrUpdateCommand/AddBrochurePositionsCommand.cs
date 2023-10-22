using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Cataloguer.Database.Models;

namespace Cataloguer.Database.Commands.AddOrUpdateCommand
{
    public class AddBrochurePositionsCommand : AbstractCommand
    {
        public AddBrochurePositionsCommand(DataBaseConfiguration config) : base(config) { }

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
    }
}
