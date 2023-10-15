using Cataloguer.Database.Commands.Base;
using Cataloguer.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Commands.GetCommands
{
    public class GetSpecialRequstCommand : AbstractCommand
    {
        public IEnumerable<Good> GetGoodsFromBrochure(int brochureId)
        {
            return Context.BrochurePositions
                .Where(x => x.BrochureId == brochureId)
                .Include(x => x.Good)
                .Select(x => x.Good!)
                .ToArray();
        }

        public IEnumerable<Distribution> GetDistributionsFromBrochure(int brochureId)
        {
            return Context.Distributions
                .Where(x => x.BrochureId == brochureId)
                .ToArray();
        }
    }
}
