using Cataloguer.Common.Models;
using Cataloguer.Common.Models.SpecialModels.OutputApiModels;
using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Commands.GetCommands;

public class GetSpecialRequestCommand : AbstractCommand
{
    public GetSpecialRequestCommand(DataBaseConfiguration config) : base(config)
    {
    }
    
    public IEnumerable<Good> GetGoodsFromBrochure(int brochureId)
    {
        // цены на товары достаются из BrochurePosition
        return Context.BrochurePositions
            .AsNoTracking()
            .Where(x => x.BrochureId == brochureId)
            .Include(x => x.Good)
            .Where(x => x.Good != null)
            .Select(x => new Good() { Name = x.Good!.Name, Price = x.Price })
            .ToArray();
    }
    
    public IEnumerable<Good> GetGoodsNotFromBrochure(int brochureId)
    {
        return Context.BrochurePositions
            .AsNoTracking()
            .Include(x => x.Good)
            .Where(x => x.BrochureId != brochureId)
            .Select(x => x.Good)
            .ToArray();
    }
    
    public IEnumerable<FrontendDistribution> GetDistributionsFromBrochure(int brochureId)
    {return Context.Distributions
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
    }

    public IEnumerable<SellHistory> GetGoodsFromSellHistory(int brochureId)
    {
        var goodsFromBrochure = Context.BrochurePositions
            .AsNoTracking()
            .Where(x => x.BrochureId == brochureId)
            .Select(x => x.Good)
            .ToList();

        return Context.SellHistory
            .AsNoTracking()
            .Include(x => x.Good)
            .Where(x => goodsFromBrochure.Contains(x.Good))
            .Include(x => x.Town)
            .Include(x => x.Gender);
    }
    
    public IEnumerable<SellHistory> GetGoodsForBrochureDistribution(int brochureId, int distributionId)
    {
        var brochure = Context.Brochures
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == brochureId);

        var distribution = Context.Distributions
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == distributionId);

        if (brochure == null || distribution == null)
            return Array.Empty<SellHistory>();

        var goodsFromBrochure = GetGoodsFromBrochure(brochureId).Select(x => x.Id).ToList();

        return Context.SellHistory
            .AsNoTracking()
            .Include(x => x.Gender)
            .Include(x => x.Good)
            .Include(x => x.Town)
            .Where(x => goodsFromBrochure.Contains(x.GoodId))
            .Where(x => x.GenderId == distribution.GenderId &&
                        x.TownId == distribution.TownId &&
                        x.Age >= distribution.AgeGroup.MinimalAge &&
                        x.Age <= distribution.AgeGroup.MaximalAge)
            .OrderBy(x => x.SellDate)
            .ToArray();
    }

    public IEnumerable<SellHistory> GetSellHistoryForBrochureGoodsAndDistributions(int brochureId)
    {
        var distributions = new GetCommand(DBConfig)
            .GetListDistribution(x => x.BrochureId == brochureId, true);

        var brochurePositions = Context.BrochurePositions
            .AsNoTracking()
            .Where(x => x.BrochureId == brochureId)
            .ToArray();

        return Context.SellHistory
            .AsNoTracking()
            .Where(x => brochurePositions.Any(y => y.GoodId == x.GoodId) &&
                distributions.Any(z => z.BrochureId == brochureId &&
                                                   z.TownId == x.TownId &&
                                                   z.GenderId == x.GenderId &&
                                                   z.AgeGroup.MinimalAge >= x.Age &&
                                                   z.AgeGroup.MaximalAge <= x.Age))
            .ToArray();
    }

    public IEnumerable<SellHistoryForChart> GetSellHistoryForChart(int brochureId)
    {
        var goodsFromBrochure = GetGoodsFromBrochure(brochureId);

        var distributions = new GetCommand(DBConfig)
            .GetListDistribution(x => x.BrochureId == brochureId, true);

        var dates = Context.SellHistory
            .AsNoTracking()
            .Select(x => x.SellDate)
            .Select(x => new DateTime(x.Year, x.Month, x.Day)) // чтобы отбросить время
            .Distinct()
            .ToArray();

        return dates.Select(x => new SellHistoryForChart()
        {
            Date = x,
            Income = Context.SellHistory
                .AsNoTracking()
                .Where(y => goodsFromBrochure.Any(z => z.Id == y.GoodId))
                .Where(y => distributions.Any(z => z.BrochureId == brochureId &&
                                                   z.TownId == y.TownId &&
                                                   z.GenderId == y.GenderId &&
                                                   z.AgeGroup.MinimalAge >= y.Age &&
                                                   z.AgeGroup.MaximalAge <= y.Age))
                .Where(y => y.SellDate.Day == x.Day &&
                            y.SellDate.Month == x.Month &&
                            y.SellDate.Year == x.Year)
                .Sum(x => x.Price)
        })
        .OrderBy(x => x.Date)
        .ToArray();
    }
}