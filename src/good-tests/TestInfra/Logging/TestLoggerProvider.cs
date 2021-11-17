using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Xunit.Abstractions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace good_tests.TestInfra.Logging
{


    // Add command handler

    /// <summary>
    /// Writes the log messages through the serilog extensions 
    /// </summary>
    public class TestLoggerProvider : IDisposable, ILoggerProvider
    {
        public static readonly string ApplicationNameProperty = "ApplicationName";

        /// <summary>
        /// This message template is denser and contains the enriched exceptions
        /// </summary>
        public static readonly string ImprovedLoggingTemplate =
            "{" + ElapsedEnricher.PropertyName + "} [{Level:u3}]{" + ApplicationNameProperty + "} "
            + "{Message} {SpanId} {ParentId} {TraceId} - {sub} {NewLine}{" +
            ExceptionFormattingEnricher.EnrichedExceptionPropertyName + "}";

        /// <summary>
        /// Use the default serilog message template if you are not happy with the improved one. 
        /// </summary>
        public static readonly string DefaultSerilogMessageTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}";

        private readonly Logger _serilog;

        public readonly LoggerFactory LoggerFactory;

        private readonly TextWriter logs = new StringWriter();
        public readonly Serilog.ILogger Serilog;

        public TestLoggerProvider(ITestOutputHelper output,
            string applicationName = null,
            string outputTemplate = null,
            LogEventLevel minimumLogLevel = LogEventLevel.Verbose,
            Func<LoggerConfiguration, LoggerConfiguration> configure = null
        )
        {
            var loggerConfiguration = new LoggerConfiguration()
                    .Enrich.With(new ElapsedEnricher())
                    .Enrich.With(new ExceptionFormattingEnricher())
                    .Enrich.FromLogContext().MinimumLevel.Is(minimumLogLevel)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                ;

            if (configure != null)
            {
                loggerConfiguration = configure.Invoke(loggerConfiguration);
            }

            if (applicationName != null)
            {
                loggerConfiguration.Enrich.WithProperty(ApplicationNameProperty,
                    " " + applicationName + " -");
            }

            _serilog = loggerConfiguration.WriteTo.TextWriter(logs, minimumLogLevel, ImprovedLoggingTemplate).WriteTo
                .TestOutput(output, minimumLogLevel, ImprovedLoggingTemplate).CreateLogger();

            Serilog = _serilog;

            LoggerFactory = new LoggerFactory();
            LoggerFactory.AddProvider(new SerilogLoggerProvider(Serilog));
        }

        public string Logs => logs.ToString();


        public void Dispose()
        {
            _serilog.Dispose();
            LoggerFactory?.Dispose();
            logs?.Dispose();
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            return LoggerFactory.CreateLogger(categoryName);
        }

        public ILogger<T> BuildLogger<T>()
        {
            return LoggerFactory.CreateLogger<T>();
        }
    }
}
