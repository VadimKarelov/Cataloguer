using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;

namespace Cataloguer.Database.Commands;

public class DeleteCommand : AbstractCommand
{
    public DeleteCommand(DataBaseConfiguration config) : base(config)
    {
    }

    public void DeleteBrochure(int id)
    {
        var brochure = Context.Brochures.FirstOrDefault(x => x.Id == id);

        if (brochure == null) return;

        Context.Remove(brochure);
        Context.SaveChanges();

        LogChange(brochure, null);
    }

    public void DeleteDistribution(int id)
    {
        var distribution = Context.Distributions.FirstOrDefault(x => x.Id == id);

        if (distribution == null) return;

        Context.Remove(distribution);
        Context.SaveChanges();

        LogChange(distribution, null);
    }

    public void DeleteGoodFromBrochure(int goodId, int brochureId)
    {
        var positionToDelete = Context.BrochurePositions
            .FirstOrDefault(x => x.BrochureId == brochureId && x.GoodId == goodId);

        if (positionToDelete == null) return;

        Context.Remove(positionToDelete);

        var brochure = Context.Brochures.FirstOrDefault(x => x.Id == brochureId);
        RememberState(brochure);
        brochure.PositionCount = Context.BrochurePositions
            .Count(x => x.BrochureId == brochureId);

        Context.Update(brochure);
        Context.SaveChanges();

        LogChange(brochure);
        LogChange(positionToDelete, null);
    }

    /// <summary>
    /// Удаляет историю прогноза для каталога, устанавливая предсказанных доход для каталога в 0
    /// </summary>
    public void DeletePredictionHistory(int brochureId)
    {
        var brochure = Context.Brochures.FirstOrDefault(x => x.Id == brochureId);

        RememberState(brochure);

        if (brochure == null) return;

        brochure.PotentialIncome = 0;

        var list = Context.PredictedSellHistory
            .Where(x => x.BrochureId == brochureId)
            .ToList();

        Context.PredictedSellHistory.RemoveRange(list);
        Context.SaveChanges();

        LogChange(brochure);

        foreach (var elem in list)
            LogChange(elem, null);
    }
}