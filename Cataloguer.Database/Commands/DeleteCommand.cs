using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;

namespace Cataloguer.Database.Commands
{
    public class DeleteCommand : AbstractCommand
    {
        public DeleteCommand(DataBaseConfiguration config) : base(config) { }

        public void DeleteBrochure(int id)
        {
            var brochure = Context.Brochures.FirstOrDefault(x => x.Id == id);

            if (brochure == null)
                return;

            Context.Remove(brochure);
            Context.SaveChanges();
        }
    }
}
