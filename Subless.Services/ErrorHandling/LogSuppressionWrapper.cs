using System;
using System.Linq;
using Serilog.Core;
using Serilog.Events;

namespace Subless.Services.ErrorHandling
{
    public class LogSuppressionWrapper : ILogEventSink, IDisposable
    {
        readonly ILogEventSink _wrappedSink;

        public LogSuppressionWrapper(ILogEventSink wrappedSink)
        {
            _wrappedSink = wrappedSink;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent.Level == LogEventLevel.Error
                 && logEvent.Exception?.Message != null
                 && logEvent.Exception.Message.Contains("IDX21324", StringComparison.Ordinal))
            {
                var boosted = new LogEvent(
                    logEvent.Timestamp,
                    LogEventLevel.Warning, // downgrade severity
                    logEvent.Exception,
                    logEvent.MessageTemplate,
                    logEvent.Properties
                        .Select(kvp => new LogEventProperty(kvp.Key, kvp.Value)));

                _wrappedSink.Emit(boosted);
            }
            else
            {
                _wrappedSink.Emit(logEvent);
            }
        }

        public void Dispose()
        {
            (_wrappedSink as IDisposable)?.Dispose();
        }
    }
}
