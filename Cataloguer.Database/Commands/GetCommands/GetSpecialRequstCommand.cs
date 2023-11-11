using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Cataloguer.Database.Models;
using Cataloguer.Database.Models.SpecialModels.OutputApiModels;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Cataloguer.Database.Commands.GetCommands
{
    public class GetSpecialRequestCommand : AbstractCommand
    {
        public GetSpecialRequestCommand(DataBaseConfiguration config) : base(config) { }

        [MethodName("получение товаров из каталога")]
        public IEnumerable<FrontendGood> GetGoodsFromBrochure(int brochureId)
        {
            StartExecuteCommand(MethodBase.GetCurrentMethod(), brochureId);

            var r = Context.BrochurePositions
                .AsNoTracking()
                .Where(x => x.BrochureId == brochureId)
                .Include(x => x.Good)
                .Where(x => x.Good != null)
                .Select(x => new FrontendGood(x.Good!) { Price = x.Price })
                .ToArray();

            FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
            return r;
        }

        [MethodName("получения рассылок из каталога")]
        public IEnumerable<FrontendDistribution> GetDistributionsFromBrochure(int brochureId)
        {
            StartExecuteCommand(MethodBase.GetCurrentMethod(), brochureId);

            var r = Context.Distributions
                .AsNoTracking()
                .Where(x => x.BrochureId == brochureId)
                .Include(x => x.Brochure)
                .Include(x => x.AgeGroup)
                .Include(x => x.Gender)
                .Include(x => x.Town)
                .Select(x => new FrontendDistribution(x)
                {
                    BrochureName = x.Brochure.Name,
                    AgeGroupName = x.AgeGroup.Description,
                    GenderName = x.Gender.Name,
                    TownName = x.Town.Name
                })
                .ToArray();

            FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
            return r;
        }

        [MethodName("получение списка товаров со средними ценами")]
        public IEnumerable<FrontendGood> GetGoodsWithAveragePriceFromHistory()
        {
            StartExecuteCommand(MethodBase.GetCurrentMethod());

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

            FinishExecuteCommand(MethodBase.GetCurrentMethod(), result);

            return result;
        }

        [MethodName("получение истории продаж товаров из каталога")]
        public IEnumerable<SellHistory> GetGoodsFromSellHistory(Brochure brochure)
        {
            StartExecuteCommand(MethodBase.GetCurrentMethod(), brochure);

            var goodsFromBrochure = Context.BrochurePositions
                .AsNoTracking()
                .Where(x => x.BrochureId == brochure.Id)
                .Select(x => x.Good)
                .ToList();

            var r = Context.SellHistory
                .AsNoTracking()
                .Include(x => x.Good)
                .Where(x => goodsFromBrochure.Contains(x.Good))
                .Include(x => x.Town)
                .Include(x => x.Gender);

            FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
            return r;
        }
    }
}
