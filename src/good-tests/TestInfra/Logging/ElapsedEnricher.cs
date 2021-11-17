using System;
using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace good_tests.TestInfra.Logging
{
    public class ElapsedEnricher : ILogEventEnricher
    {
        public const string PropertyName = "Elapsed";
        private readonly Stopwatch _stopwatch;

        public ElapsedEnricher()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var value = (int)Math.Floor(_stopwatch.Elapsed.TotalSeconds) + "." + _stopwatch.Elapsed.Milliseconds;
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(PropertyName,
                value));
        }
    }
}