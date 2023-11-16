using System.Reflection;
using Cataloguer.Common.Models;
using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Commands.GetCommands;

public class GetCommand : AbstractCommand
{
    public GetCommand(DataBaseConfiguration configuration) : base(configuration)
    {
    }

    [MethodName("получение возрастной группы")]
    public AgeGroup? GetAgeGroup(int id)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), id);

        var r = Context.AgeGroups
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);

        return r;
    }

    [MethodName("получение списка возрастных групп")]
    public IEnumerable<AgeGroup> GetListAgeGroup(Func<AgeGroup, bool>? predicate = null)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), predicate);

        var r = TryApplyPredicate(Context.AgeGroups.AsNoTracking(), predicate).ToArray();

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    [MethodName("получение каталога")]
    public Brochure? GetBrochure(int id, bool includeFields = false)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), id, includeFields);

        var r = includeFields
            ? Context.Brochures
                .AsNoTracking()
                .Include(x => x.Status)
                .FirstOrDefault(x => x.Id == id)
            : Context.Brochures
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    [MethodName("получение списка каталогов")]
    public IEnumerable<Brochure> GetListBrochure(Func<Brochure, bool>? predicate = null, bool includeFields = false)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), predicate, includeFields);

        var request = Context.Brochures
            .AsNoTracking()
            .OrderByDescending(x => x.Id);

        if (includeFields) request.Include(x => x.Status);

        var r = TryApplyPredicate(request, predicate);

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    [MethodName("получение позиции каталога")]
    public BrochurePosition? GetBrochurePosition(int id, bool includeFields = false)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), id, includeFields);

        var r = includeFields
            ? Context.BrochurePositions
                .AsNoTracking()
                .Include(x => x.Brochure)
                .Include(x => x.Good)
                .FirstOrDefault(x => x.Id == id)
            : Context.BrochurePositions
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    [MethodName("получение списка позиций каталогов")]
    public IEnumerable<BrochurePosition> GetListBrochurePositions(Func<BrochurePosition, bool>? predicate = null,
        bool includeFields = false)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), predicate, includeFields);

        var request = Context.BrochurePositions.AsNoTracking();

        if (includeFields)
            request.Include(x => x.Brochure)
                .Include(x => x.Good);

        var r = TryApplyPredicate(request, predicate);

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    [MethodName("получение рассылки")]
    public Distribution? GetDistribution(int id, bool includeFields = false)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), id, includeFields);

        var r = includeFields
            ? Context.Distributions
                .AsNoTracking()
                .Include(x => x.Town)
                .Include(x => x.AgeGroup)
                .Include(x => x.Brochure)
                .Include(x => x.Gender)
                .FirstOrDefault(x => x.Id == id)
            : Context.Distributions
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    [MethodName("получение списка рассылок")]
    public IEnumerable<Distribution> GetListDistribution(Func<Distribution, bool>? predicate = null,
        bool includeFields = false)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), predicate, includeFields);

        var request = Context.Distributions.AsNoTracking();

        if (includeFields)
            request.Include(x => x.Town)
                .Include(x => x.AgeGroup)
                .Include(x => x.Brochure)
                .Include(x => x.Gender);

        var r = TryApplyPredicate(request, predicate);

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    [MethodName("получение пола")]
    public Gender? GetGender(int id)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), id);

        var r = Context.Genders
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    [MethodName("получение списка полов")]
    public IEnumerable<Gender> GetListGender(Func<Gender, bool>? predicate = null)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), predicate);

        var r = TryApplyPredicate(Context.Genders.AsNoTracking(), predicate).ToArray();

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    [MethodName("получение товара")]
    public Good? GetGood(int id)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), id);

        var r = Context.Goods
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    [MethodName("получение списка товаров")]
    public IEnumerable<Good> GetListGood(Func<Good, bool>? predicate = null)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), predicate);

        var r = TryApplyPredicate(Context.Goods.AsNoTracking(), predicate).ToArray();

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    [MethodName("получение записи истории покупок")]
    public SellHistory? GetSellHistory(int id, bool includeFields = false)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), id, includeFields);

        var r = includeFields
            ? Context.SellHistory
                .AsNoTracking()
                .Include(x => x.Town)
                .Include(x => x.Good)
                .Include(x => x.Gender)
                .FirstOrDefault(x => x.Id == id)
            : Context.SellHistory
                .FirstOrDefault(x => x.Id == id);

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    [MethodName("получение всей истории покупок")]
    public IEnumerable<SellHistory> GetListSellHistory(Func<SellHistory, bool>? predicate = null,
        bool includeFields = false)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), predicate, includeFields);

        var request = Context.SellHistory.AsNoTracking();

        if (includeFields)
            request.Include(x => x.Town)
                .Include(x => x.Good)
                .Include(x => x.Gender);

        var r = TryApplyPredicate(request, predicate);

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    [MethodName("получение статуса")]
    public Status? GetStatus(int id)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), id);

        var r = Context.Statuses
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    [MethodName("получение списка статусов")]
    public IEnumerable<Status> GetListStatus(Func<Status, bool>? predicate = null)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), predicate);

        var r = TryApplyPredicate(Context.Statuses.AsNoTracking(), predicate).ToArray();

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    [MethodName("получение города")]
    public Town? GetTown(int id)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), id);

        var r = Context.Towns
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    [MethodName("получение списка городов")]
    public IEnumerable<Town> GetListTown(Func<Town, bool>? predicate = null)
    {
        StartExecuteCommand(MethodBase.GetCurrentMethod(), predicate);

        var r = TryApplyPredicate(Context.Towns.AsNoTracking(), predicate).ToArray();

        FinishExecuteCommand(MethodBase.GetCurrentMethod(), r);
        return r;
    }

    private static IEnumerable<T> TryApplyPredicate<T>(IQueryable<T> request, Func<T, bool>? predicate)
    {
        return predicate is null ? request.ToArray() : request.Where(predicate).ToArray();
    }
}