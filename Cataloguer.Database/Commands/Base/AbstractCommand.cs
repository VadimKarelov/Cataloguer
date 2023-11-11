using Cataloguer.Database.Base;
using Serilog;
using System.Reflection;
using System.Text;
using System.Text.Json;

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

        /// <summary>
        /// Обработка действий перед началом выполнения команды
        /// </summary>
        protected void StartExecuteCommand(string commandDescription, params object?[] inputParams)
        {
            string parameters = new StringBuilder().AppendJoin(' ', inputParams.Select(x => JsonSerializer.Serialize(x))).ToString();
            Log.Information($"Начало выполнения команды <{commandDescription}>. Входные параметры: {parameters}");
        }

        /// <summary>
        /// Обработка действий перед началом выполнения команды
        /// </summary>
        /// <param name="currentMethod">Получается через System.Reflection.MethodBase.GetCurrentMethod(). Метод требует атрибут MethodNameAttribute.</param>
        protected void StartExecuteCommand(MethodBase? currentMethod, params object?[] inputParams)
        {
            GetMethodInfo(currentMethod, out string commandDescription);

            StartExecuteCommand(commandDescription, inputParams);
        }

        /// <summary>
        /// Обработка действий после выполнения команды
        /// </summary>
        protected void FinishExecuteCommand(string commandDescription, params object?[] outputParams)
        {
            string parameters = new StringBuilder().AppendJoin(' ', outputParams.Select(x => JsonSerializer.Serialize(x))).ToString();
            Log.Information($"Выполнена команда <{commandDescription}>. Результат: {parameters}");
        }

        /// <summary>
        /// Обработка действий перед началом выполнения команды
        /// </summary>
        /// <param name="currentMethod">Получается через System.Reflection.MethodBase.GetCurrentMethod(). Метод требует атрибут MethodNameAttribute.</param>
        protected void FinishExecuteCommand(MethodBase? currentMethod, params object?[] outputParams)
        {
            GetMethodInfo(currentMethod, out string commandDescription);

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
}
