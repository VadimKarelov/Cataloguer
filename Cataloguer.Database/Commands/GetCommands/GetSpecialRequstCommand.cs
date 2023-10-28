﻿using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Cataloguer.Database.Models;
using Cataloguer.Database.Models.SpecialModels;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Commands.GetCommands
{
    public class GetSpecialRequestCommand : AbstractCommand
    {
        public GetSpecialRequestCommand(DataBaseConfiguration config) : base(config) { }

        public IEnumerable<Good> GetGoodsFromBrochure(int brochureId)
        {
            return Context.BrochurePositions
                .AsNoTracking()
                .Where(x => x.BrochureId == brochureId)
                .Include(x => x.Good)
                .Select(x => x.Good!)
                .ToArray();
        }

        public IEnumerable<Distribution> GetDistributionsFromBrochure(int brochureId)
        {
            return Context.Distributions
                .AsNoTracking()
                .Where(x => x.BrochureId == brochureId)
                .ToArray();
        }

        public IEnumerable<FrontendGood> GetGoodsWithAveragePriceFromHistory()
        {
            var sellHistory = Context.SellHistory
                .AsNoTracking()
                .ToList();

            var goods = Context.Goods
                .AsNoTracking()
                .ToList();

            var result = new List<FrontendGood>();

            foreach (var good in goods)
            {
                var purchases = Context.SellHistory
                    .AsNoTracking()
                    .Where(x => x.GoodId == good.Id);

                decimal avgPrice = 0;

                if (purchases.Any())
                    avgPrice = Math.Round(purchases.Average(x => x.Price), 2);

                result.Add(new FrontendGood(good) { Price = avgPrice });
            }

            return result;
        }

        public IEnumerable<SellHistory> GetGoodsFromSellHistory(Brochure brochure)
        {
            var goodsFromBrochure = Context.BrochurePositions
                .AsNoTracking()
                .Where(x => x.BrochureId == brochure.Id)
                .Select(x => x.Good)
                .ToList();

            return Context.SellHistory
                .AsNoTracking()
                .Include(x => x.Good)
                .Where(x => goodsFromBrochure.Contains(x.Good))
                .Include(x => x.Town)
                .Include(x => x.Gender);
        }
    }
}
