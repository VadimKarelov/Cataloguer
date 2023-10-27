﻿using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Cataloguer.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Commands.GetCommands
{
    public class GetCommand : AbstractCommand
    {
        public GetCommand(DataBaseConfiguration configuration) : base(configuration) { }

        public AgeGroup? GetAgeGroup(int id)
        {
            return Context.AgeGroups
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<AgeGroup> GetListAgeGroup(Func<AgeGroup, bool>? predicate = null)
        {
            return TryApplyPredicate(Context.AgeGroups.AsNoTracking(), predicate).ToArray();
        }

        public Brochure? GetBrochure(int id, bool includeFields = false)
        {
            return includeFields ?
                Context.Brochures
                    .AsNoTracking()
                    .Include(x => x.Status)
                    .FirstOrDefault(x => x.Id == id) :
                Context.Brochures
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Brochure> GetListBrochure(Func<Brochure, bool>? predicate, bool includeFields = false)
        {
            var request = Context.Brochures.AsNoTracking();

            if (includeFields)
            {
                request.Include(x => x.Status);
            }

            return TryApplyPredicate(request, predicate);
        }

        public BrochurePosition? GetBrochurePosition(int id, bool includeFields = false)
        {
            return includeFields ?
                Context.BrochurePositions
                    .AsNoTracking()
                    .Include(x => x.Brochure)
                    .Include(x => x.Good)
                    .FirstOrDefault(x => x.Id == id) :
                Context.BrochurePositions
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<BrochurePosition> GetListBrochurePositions(Func<BrochurePosition, bool>? predicate, bool includeFields = false)
        {
            var request = Context.BrochurePositions.AsNoTracking();

            if (includeFields)
            {
                request.Include(x => x.Brochure)
                    .Include(x => x.Good);
            }

            return TryApplyPredicate(request, predicate);
        }

        public Distribution? GetDistribution(int id, bool includeFields = false)
        {
            return includeFields ?
                Context.Distributions
                    .AsNoTracking()
                    .Include(x => x.Town)
                    .Include(x => x.AgeGroup)
                    .Include(x => x.Brochure)
                    .Include(x => x.Gender)
                    .FirstOrDefault(x => x.Id == id) :
                Context.Distributions
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Distribution> GetListDistribution(Func<Distribution, bool>? predicate, bool includeFields = false)
        {
            var request = Context.Distributions.AsNoTracking();

            if (includeFields)
            {
                request.Include(x => x.Town)
                    .Include(x => x.AgeGroup)
                    .Include(x => x.Brochure)
                    .Include(x => x.Gender);
            }

            return TryApplyPredicate(request, predicate);
        }

        public Gender? GetGender(int id)
        {
            return Context.Genders
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Gender> GetListGender(Func<Gender, bool>? predicate = null)
        {
            return TryApplyPredicate(Context.Genders.AsNoTracking(), predicate).ToArray();
        }

        public Good? GetGood(int id)
        {
            return Context.Goods
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Good> GetListGood(Func<Good, bool>? predicate = null)
        {
            return TryApplyPredicate(Context.Goods.AsNoTracking(), predicate).ToArray();
        }

        public SellHistory? GetSellHistory(int id, bool includeFields = false)
        {
            return includeFields ?
                Context.SellHistory
                    .AsNoTracking()
                    .Include(x => x.Town)
                    .Include(x => x.Good)
                    .Include(x => x.Gender)
                    .FirstOrDefault(x => x.Id == id) :
                Context.SellHistory
                    .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<SellHistory> GetListSellHistory(Func<SellHistory, bool>? predicate, bool includeFields = false)
        {
            var request = Context.SellHistory.AsNoTracking();

            if (includeFields)
            {
                request.Include(x => x.Town)
                    .Include(x => x.Good)
                    .Include(x => x.Gender);
            }

            return TryApplyPredicate(request, predicate);
        }

        public Status? GetStatus(int id)
        {
            return Context.Statuses
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Status> GetListStatus(Func<Status, bool>? predicate = null)
        {
            return TryApplyPredicate(Context.Statuses.AsNoTracking(), predicate).ToArray();
        }

        public Town? GetTown(int id)
        {
            return Context.Towns
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Town> GetListTown(Func<Town, bool>? predicate = null)
        {
            return TryApplyPredicate(Context.Towns.AsNoTracking(), predicate).ToArray();
        }

        private static IEnumerable<T> TryApplyPredicate<T>(IQueryable<T> request, Func<T, bool>? predicate)
        {
            return predicate is null ? request.AsEnumerable() : request.Where(predicate);
        }
    }
}
