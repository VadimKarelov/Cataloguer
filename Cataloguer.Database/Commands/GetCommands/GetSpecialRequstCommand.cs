﻿using Cataloguer.Database.Commands.Base;
using Cataloguer.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Commands.GetCommands
{
    public class GetSpecialRequstCommand : AbstractCommand
    {
        public IEnumerable<Good> GetGoodsFromBrochure(int brochureId)
        {
            var positions = Context.BrochurePositions
                .Where(x => x.BrochureId == brochureId)
                .Include(x => x.Good);

            return positions.Select(x => x.Good!).ToArray();
        }
    }
}
