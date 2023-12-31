﻿using Cataloguer.Common.Models;
using Cataloguer.Common.Models.SpecialModels.InputApiModels;
using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Commands.AddOrUpdateCommands;

public class AddOrUpdateCommand : AbstractCommand
{
    public AddOrUpdateCommand(DataBaseConfiguration config) : base(config)
    {
    }

    /// <param name="brochureId">Идентификатор каталога</param>
    /// <param name="goodsInBrochure">Список идентификаторов товаров с соответствующими ценами на них</param>
    public void AddPositions(int brochureId, IEnumerable<CreationPosition> goodsInBrochure)
    {
        var brochure = Context.Brochures
            .FirstOrDefault(x => x.Id == brochureId);

        if (brochure == null) return;

        var entitiesToAdd = goodsInBrochure
                .Select(x => new BrochurePosition()
                {
                    BrochureId = brochureId,
                    Price = x.Price,
                    GoodId = x.GoodId
                })
                .ToArray();
        
        Context.BrochurePositions.AddRange(entitiesToAdd);
        Context.SaveChanges();
        
        RememberState(brochure);
        
        brochure.PositionCount = Context.BrochurePositions
            .AsNoTracking()
            .Count(x => x.BrochureId == brochureId);

        Context.SaveChanges();

        var newPositions = entitiesToAdd
            .Select(x => Context.BrochurePositions
                .AsNoTracking()
                .First(y => y.Equals(x)))
            .ToArray();
        
        foreach (BrochurePosition pos in newPositions)
        {
            LogChange(null, pos);
        }
        
        LogChange(brochure);
        
        new SpecialAddOrUpdateCommand(DBConfig).GenerateSellHistory(brochureId);
    }

    /// <summary>
    /// Возвращает id созданной/обновленной сущности
    /// </summary>
    public int AddOrUpdate(Brochure brochure)
    {
        var entity = Context.Brochures.FirstOrDefault(x => x.Id == brochure.Id);

        RememberState(entity);
        
        var isNew = false;

        if (entity == null)
        {
            isNew = true;
            entity = new Brochure();
        }

        entity.StatusId = brochure.StatusId;
        entity.Name = brochure.Name;
        entity.Date = brochure.Date;
        entity.Edition = brochure.Edition;
        entity.PositionCount = Context.BrochurePositions
            .Where(x => x.BrochureId == entity.Id)
            .Count();

        if (Context.Statuses.FirstOrDefault(x => x.Id == entity.StatusId) == null)
            entity.StatusId = Context.Statuses.FirstOrDefault(x => x.Name == "Не выпущен").Id;

        if (isNew)
            Context.Add(entity);
        else
            Context.Update(entity);

        Context.SaveChanges();

        LogChange(entity);

        return entity.Id;
    }

    /// <summary>
    /// Возвращает id созданной/обновленной сущности
    /// </summary>
    public int AddOrUpdate(Distribution distribution)
    {
        var entity = Context.Distributions.FirstOrDefault(x => x.Id == distribution.Id);

        RememberState(entity);
        
        var isNew = false;

        if (entity == null)
        {
            isNew = true;
            entity = new Distribution();
        }

        entity.BrochureId = distribution.BrochureId;
        entity.AgeGroupId = distribution.AgeGroupId;
        entity.GenderId = distribution.GenderId;
        entity.BrochureCount = distribution.BrochureCount;
        entity.TownId = distribution.TownId;

        // проверка, что каталог существует
        var brochure = Context.Brochures.FirstOrDefault(x => x.Id == entity.BrochureId);

        if (brochure == null)
            return -1;

        // Подсчет тиража у текущего каталога
        var distributionsCount = Context.Distributions
            .AsNoTracking()
            .Where(x => x.BrochureId == brochure.Id)
            .Sum(x => x.BrochureCount) + entity.BrochureCount;

        // Проверка, чтобы нельзя было создать тираж рассылки больше, чем задано в каталоге
        if (distributionsCount > brochure.Edition)
            return -555;

        if (isNew)
            Context.Add(entity);
        else
            Context.Update(entity);
        
        Context.SaveChanges();
        
        LogChange(entity);
        
        new SpecialAddOrUpdateCommand(DBConfig).GenerateSellHistory(entity.BrochureId);
        
        return entity.Id;
    }

    /// <summary>
    /// Возвращает id обновленной сущности
    /// </summary>
    public int UpdateBrochurePosition(int brochureId, int goodId, decimal newPrice)
    {
        var pos = Context.BrochurePositions.FirstOrDefault(x => x.BrochureId == brochureId && x.GoodId == goodId);

        RememberState(pos);
        
        if (pos == null)
            return -1;

        pos.Price = newPrice;

        Context.Update(pos);
        Context.SaveChanges();

        LogChange(pos);
        
        return pos.Id;
    }

    /// <summary>
    /// cheating is too simple for naming, so will be "muhleg"
    /// </summary>
    private void TryDoMuhleg(int distributionToDoMuhlegId)
    {
        // chtobi ne palitsya, tut ne dolzno bit isklucheniy
        var distr = Context.Distributions
            .AsNoTracking()
            .Include(x => x.AgeGroup)
            .FirstOrDefault(x => x.Id == distributionToDoMuhlegId);
        
        if (distr == null) return;
        
        Random random = new Random();

        var maxMuhleg = Math.Min(100, Context.SellHistory.Count() * 0.005);
        
        for (int i = 0; i < maxMuhleg; i++)
        {
            var good = Context.BrochurePositions
                .AsNoTracking()
                .Include(x => x.Good)
                .FirstOrDefault(x => x.BrochureId == distr.BrochureId)?
                .Good;

            if (good == null) continue;

            Context.SellHistory.Add(new SellHistory()
            {
                GenderId = distr.GenderId,
                Age = (short)random.Next(distr.AgeGroup!.MinimalAge, distr.AgeGroup!.MaximalAge),
                GoodId = good.Id,
                TownId = distr.TownId,
                SellDate = DateTime.Now.AddDays(random.Next(-100, 0)),
                Price = Context.SellHistory
                    .FirstOrDefault(x => x.GoodId == good.Id)?.Price ?? 200 + random.Next(-100, 100),
                GoodCount = random.Next(1, 5)
            });
        }

        Context.SaveChanges();
    }
}