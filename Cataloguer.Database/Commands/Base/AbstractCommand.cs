using Cataloguer.Database.Base;

namespace Cataloguer.Database.Commands.Base
{
    public abstract class AbstractCommand
    {
        private protected CataloguerApplicationContext Context { get; set; }

        public AbstractCommand(DataBaseConfiguration config)
        {
            Context = new CataloguerApplicationContext(config);
            Context.Init();
        }
    }
}
