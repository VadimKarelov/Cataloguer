using System.Reflection;
using System.Text;
using System.Text.Json;
using Cataloguer.Common.Models.SpecialModels.Logging;
using Cataloguer.Common.Models;
using Cataloguer.Database.Base;
using Serilog;

namespace Cataloguer.Database.Commands.Base;

public abstract class AbstractCommand
{
    protected DataBaseConfiguration DBConfig { get; private set; }
    
    public AbstractCommand(DataBaseConfiguration config)
    {
        Context = new CataloguerApplicationContext(config);
        DBConfig = config;
        Context.Init();
    }

    private protected CataloguerApplicationContext Context { get; set; }

    /// <summary>
    /// Обработка действий перед началом выполнения команды
    /// </summary>
    protected void StartExecuteCommand(string commandDescription, params object?[] inputParams)
    {
        var parameters = ConvertParametersToString(inputParams);
        Log.Information($"Начало выполнения команды <{commandDescription}>. Входные параметры: {parameters}");
    }

    /// <summary>
    /// Обработка действий перед началом выполнения команды
    /// </summary>
    /// <param name="currentMethod">
    /// Получается через System.Reflection.MethodBase.GetCurrentMethod(). Метод требует атрибут
    /// MethodNameAttribute.
    /// </param>
    protected void StartExecuteCommand(MethodBase? currentMethod, params object?[] inputParams)
    {
        GetMethodInfo(currentMethod, out var commandDescription);

        StartExecuteCommand(commandDescription, inputParams);
    }

    /// <summary>
    /// Обработка действий после выполнения команды
    /// </summary>
    protected void FinishExecuteCommand(string commandDescription, params object?[] outputParams)
    {
        var parameters = ConvertParametersToString(outputParams);
        Log.Information($"Выполнена команда <{commandDescription}>. Результат: {parameters}");
    }

    /// <summary>
    /// Обработка действий перед началом выполнения команды
    /// </summary>
    /// <param name="currentMethod">
    /// Получается через System.Reflection.MethodBase.GetCurrentMethod(). Метод требует атрибут
    /// MethodNameAttribute.
    /// </param>
    protected void FinishExecuteCommand(MethodBase? currentMethod, params object?[] outputParams)
    {
        GetMethodInfo(currentMethod, out var commandDescription);

        FinishExecuteCommand(commandDescription, outputParams);
    }

    // todo Идеально добавить получение информации по входящим параметрам (пока не нашел)
    private void GetMethodInfo(MethodBase? currentMethod, out string commandDescription)
    {
        if (currentMethod == null) throw new ArgumentNullException(nameof(currentMethod));

        var requiredAttribute = currentMethod.GetCustomAttributes().FirstOrDefault(x => x is MethodNameAttribute);

        commandDescription = (requiredAttribute as MethodNameAttribute)?.MethodName ?? string.Empty;
    }

    private string ConvertParametersToString(params object?[] parameters)
    {
        return new StringBuilder().AppendJoin(' ', parameters
                .Where(x => x is not null && 
                            x.GetType() != typeof(Delegate) && 
                            x.GetType() != typeof(Func<>) &&
                            x.GetType() != typeof(Action) &&
                            x.GetType() != typeof(System.Func<Distribution, bool>))
                .Select(x => JsonSerializer.Serialize(x)))
            .ToString();
    }

    protected void LogChange(object? previousObjectState, object? newObjectState)
    {
        if (previousObjectState == null && newObjectState == null) return;

        if (previousObjectState != null && newObjectState != null && previousObjectState.GetType() != newObjectState.GetType()) return;

        var typeName = previousObjectState?.GetType().Name ?? newObjectState?.GetType().Name;

        var prevProperties = GetProperties(previousObjectState);

        var newProperties = GetProperties(newObjectState);

        // выьираем любую не пустую коллекцию свойств
        var notNullCollection = prevProperties ?? newProperties;

        foreach (var prop in notNullCollection!)
        {

        }

        Context.SaveChanges();
    }

    private Dictionary<string, object?>? GetProperties(object? obj)
    {
        return obj?.GetType()
                .GetProperties()
                .ToDictionary(p => p.Name,
                p => p.GetValue(obj));
    }
}