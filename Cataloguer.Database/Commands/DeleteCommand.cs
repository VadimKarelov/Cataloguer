using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using System.Reflection;

namespace Cataloguer.Database.Commands
{
    public class DeleteCommand : AbstractCommand
    {
        public DeleteCommand(DataBaseConfiguration config) : base(config) { }

        [MethodName("удаление каталога")]
        public void DeleteBrochure(int id)
        {
            StartExecuteCommand(MethodBase.GetCurrentMethod(), id);

            var brochure = Context.Brochures.FirstOrDefault(x => x.Id == id);

            if (brochure == null) return;

            Context.Remove(brochure);
            Context.SaveChanges();

            FinishExecuteCommand(MethodBase.GetCurrentMethod());
        }

        [MethodName("удаление рассылки")]
        public void DeleteDistribution(int id)
        {
            StartExecuteCommand(MethodBase.GetCurrentMethod(), id);

            var distribution = Context.Distributions.FirstOrDefault(x => x.Id == id);

            if (distribution == null) return;

            Context.Remove(distribution);
            Context.SaveChanges();

            FinishExecuteCommand(MethodBase.GetCurrentMethod());
        }

        [MethodName("удаление товара из каталога")]
        public void DeleteGoodFromBrochure(int brochureId, int goodId)
        {
            StartExecuteCommand(MethodBase.GetCurrentMethod(), brochureId, goodId);

            var positionToDelete = Context.BrochurePositions
                .FirstOrDefault(x => x.BrochureId == brochureId && x.GoodId == goodId);

            if (positionToDelete == null) return;

            Context.Remove(positionToDelete);
            Context.SaveChanges();

            FinishExecuteCommand(MethodBase.GetCurrentMethod());
        }
    }
}
