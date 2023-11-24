using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
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

    [MethodName("получение товаров из каталога")]
    public IEnumerable<Good> GetGoodsFromBrochure(int brochureId)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), brochureId);

        // цены на товары достаются из BrochurePostion
        var r = Context.BrochurePositions
            .AsNoTracking()
            .Where(x => x.BrochureId == brochureId)
            .Include(x => x.Good)
            .Where(x => x.Good != null)
            .Select(x => new Good() { Name = x.Good!.Name, Price = x.Price })
            .ToArray();

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    [MethodName("получение товаров, которых нет в каталоге")]
    public IEnumerable<Good> GetGoodsNotFromBrochure(int brochureId)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), brochureId);

        var r = Context.BrochurePositions
            .AsNoTracking()
            .Include(x => x.Good)
            .Where(x => x.BrochureId != brochureId)
            .Select(x => x.Good)
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

    [MethodName("получение истории покупок по рассылке")]
    public IEnumerable<SellHistory> GetGoodsForBrochureDistribution(int brochureId, int distributionId)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), brochureId, distributionId);

        var brochure = Context.Brochures
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == brochureId);

        var distribution = Context.Distributions
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == distributionId);

        if (brochure == null || distribution == null)
            return Array.Empty<SellHistory>();

        var goodsFromBrochure = GetGoodsFromBrochure(brochureId).Select(x => x.Id).ToList();

        var r = Context.SellHistory
            .AsNoTracking()
            .Include(x => x.Gender)
            .Include(x => x.Good)
            .Include(x => x.Town)
            .Where(x => goodsFromBrochure.Contains(x.GoodId))
            .Where(x => x.GenderId == distribution.GenderId &&
                        x.TownId == distribution.TownId &&
                        x.Age >= distribution.AgeGroup.MinimalAge &&
                        x.Age <= distribution.AgeGroup.MaximalAge)
            .ToArray();
        
        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    public IEnumerable<SellHistoryForChart> GetSellHistoryForChart()
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod());

        var dates = Context.SellHistory
            .AsNoTracking()
            .Select(x => x.SellDate)
            .Select(x => new DateTime(x.Year, x.Month, x.Day)) // чтобы отбросить время
            .Distinct()
            .ToArray();

        var r = dates.Select(x => new SellHistoryForChart()
        {
            Date = x,
            Income = Context.SellHistory
                .AsNoTracking()
                .Where(y => y.SellDate.Day == x.Day &&
                            y.SellDate.Month == x.Month &&
                            y.SellDate.Year == x.Year)
                .Sum(x => x.Price)
        })
        .OrderBy(x => x.Date)
        .ToArray();
        
        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }
}