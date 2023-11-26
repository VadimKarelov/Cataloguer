﻿using System.Reflection;
using System.Text;
using System.Text.Json;
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
    ///     Обработка действий перед началом выполнения команды
    /// </summary>
    protected void StartExecuteCommand(string commandDescription, params object?[] inputParams)
    {
        var parameters = new StringBuilder().AppendJoin(' ', inputParams.Select(x => JsonSerializer.Serialize(x)))
            .ToString();
        Log.Information($"Начало выполнения команды <{commandDescription}>. Входные параметры: {parameters}");
    }

    /// <summary>
    ///     Обработка действий перед началом выполнения команды
    /// </summary>
    /// <param name="currentMethod">
    ///     Получается через System.Reflection.MethodBase.GetCurrentMethod(). Метод требует атрибут
    ///     MethodNameAttribute.
    /// </param>
    protected void StartExecuteCommand(MethodBase? currentMethod, params object?[] inputParams)
    {
        GetMethodInfo(currentMethod, out var commandDescription);

        StartExecuteCommand(commandDescription, inputParams);
    }

    /// <summary>
    ///     Обработка действий после выполнения команды
    /// </summary>
    protected void FinishExecuteCommand(string commandDescription, params object?[] outputParams)
    {
        var parameters = new StringBuilder().AppendJoin(' ', outputParams.Select(x => JsonSerializer.Serialize(x)))
            .ToString();
        Log.Information($"Выполнена команда <{commandDescription}>. Результат: {parameters}");
    }

    /// <summary>
    ///     Обработка действий перед началом выполнения команды
    /// </summary>
    /// <param name="currentMethod">
    ///     Получается через System.Reflection.MethodBase.GetCurrentMethod(). Метод требует атрибут
    ///     MethodNameAttribute.
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
}