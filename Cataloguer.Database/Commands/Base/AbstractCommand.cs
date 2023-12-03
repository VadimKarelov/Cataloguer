using System.Reflection;
using System.Text.Json.Serialization;
using Cataloguer.Common.Models.SpecialModels.Logging;
using Cataloguer.Database.Base;

namespace Cataloguer.Database.Commands.Base;

public abstract class AbstractCommand
{
    private protected DataBaseConfiguration DBConfig { get; private set; }
    
    private protected CataloguerApplicationContext Context { get; set; }

    private object? _rememberedState;
    
    public AbstractCommand(DataBaseConfiguration config)
    {
        Context = new CataloguerApplicationContext(config);
        DBConfig = config;
        Context.Init();
    }

    protected void RememberState(object? objectToRemember)
    {
        _rememberedState = ShallowCopy(objectToRemember);
    }

    /// <summary>
    ///     Вызывается только после вызова RememberState
    /// </summary>
    protected void LogChange(object? newState)
    {
        LogChange(_rememberedState, newState);
    }
    
    protected void LogChange(object? previousObjectState, object? newObjectState)
    {
        if (previousObjectState == null && newObjectState == null) return;

        if (previousObjectState != null && newObjectState != null && previousObjectState.GetType() != newObjectState.GetType()) return;

        var typeName = previousObjectState?.GetType().Name ?? newObjectState?.GetType().Name;

        var prevProperties = GetProperties(previousObjectState);

        var newProperties = GetProperties(newObjectState);

        var entityId = int.Parse(
            newProperties?.FirstOrDefault(x => x.Name == "Id").GetValue(newObjectState).ToString() ??
            prevProperties?.FirstOrDefault(x => x.Name == "Id").GetValue(previousObjectState).ToString());
        
        // выбираем любую не пустую коллекцию свойств
        var notNullCollection = prevProperties ?? newProperties;

        foreach (var prop in notNullCollection!)
        {
            // игнорируем поля, помеченные для игнорирования сериализацией
            if (prop.GetCustomAttributes().Any(a => a.GetType() is JsonIgnoreAttribute)) continue;

            var prevValue = previousObjectState != null ? prop.GetValue(previousObjectState) : null;
            var newValue = newObjectState != null ? prop.GetValue(newObjectState) : null;
            
            if (prevValue == null && newValue == null || 
                prevValue != null && newValue != null && prevValue.Equals(newValue)) continue;
            
            LogEntity log = new LogEntity()
            {
                DateTime = DateTime.Now,
                TypeName = typeName ?? "Не определено",
                EntityId = entityId,
                PropertyName = prop.Name,
                PreviousValue = prevValue?.ToString(),
                NewValue = newValue?.ToString()
            };

            Context.Logs.Add(log);
        }

        Context.SaveChanges();
    }

    private PropertyInfo[]? GetProperties(object? obj)
    {
        return obj?.GetType()?.GetProperties();
    }

    private object? ShallowCopy(object? copyFrom)
    {
        if (copyFrom == null) return null;

        var res = Activator.CreateInstance(copyFrom.GetType());

        foreach (var prop in copyFrom.GetType().GetProperties())
        {
            prop.SetValue(res, prop.GetValue(copyFrom));
        }

        return res;
    }
}