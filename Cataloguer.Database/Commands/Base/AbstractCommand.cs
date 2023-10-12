using Cataloguer.Database.Base;

namespace Cataloguer.Database.Commands.Base
{
    public abstract class AbstractCommand
    {
        private protected CataloguerApplicationContext Context { get; set; }

        public AbstractCommand()
        {
            Context = new CataloguerApplicationContext();
            Context.Init();
        }
    }
}
