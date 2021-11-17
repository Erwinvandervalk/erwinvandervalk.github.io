using System;
using System.Collections;
using System.Text;
using Serilog.Core;
using Serilog.Events;

namespace good_tests.TestInfra.Logging
{
    /// <summary>
    ///     Adds a formatted version of an exception as a property, which then can easily be pretty-printed.
    /// </summary>
    /// <seealso cref="Serilog.Core.ILogEventEnricher" />
    public class ExceptionFormattingEnricher : ILogEventEnricher
    {
        public const string EnrichedExceptionPropertyName = "_detailedException";

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Exception != null)
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(EnrichedExceptionPropertyName,
                    FormatException(logEvent.Exception)));
        }

        private static string FormatData(IDictionary dict, string indent)
        {
            if (dict == null) return string.Empty;

            indent = indent + "    ";
            var retVal = "";
            foreach (var val in dict.Keys) retVal += $"{Environment.NewLine}{indent}'{val}' : '{dict[val]}'";

            return retVal;
        }


        public static string FormatException(Exception arg)
        {
            var builder = new StringBuilder();
            FormatException(arg, builder);
            return builder.ToString();
        }

        private static void FormatException(Exception exception, StringBuilder logs, int tabs = 4)
        {
            if (exception == null) return;

            var indent = new string(' ', tabs);

            logs.AppendLine().AppendLine($"{indent}{exception.GetType()?.Name}:")
                .AppendLine($"{indent}Message    : '{exception.Message}");

            if (exception.Data.Count > 0) logs.AppendLine($"{indent}Data       : {FormatData(exception.Data, indent)}");

            logs.AppendLine(
                    $"{indent}StackTrace : '{Environment.NewLine}{indent}    {exception.StackTrace?.Replace(Environment.NewLine, Environment.NewLine + indent + "    ")}")
                .AppendLine($"{indent}TargetSite : '{exception.TargetSite}");

            if (exception.InnerException != null) logs.AppendLine($"{indent}InnerEx    : ");

            FormatException(exception.InnerException, logs, tabs + 4);
        }
    }
}