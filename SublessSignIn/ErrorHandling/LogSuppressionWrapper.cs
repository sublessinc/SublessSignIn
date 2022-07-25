using System;
using System.Linq;
using Serilog.Core;
using Serilog.Events;

namespace SublessSignIn.ErrorHandling
{
    public class LogSuppressionWrapper : ILogEventSink, IDisposable
    {
        private readonly ILogEventSink _wrappedSink;

        public LogSuppressionWrapper(ILogEventSink wrappedSink)
        {
            _wrappedSink = wrappedSink;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent.Level==LogEventLevel.Error && logEvent.MessageTemplate.Text.Contains("An error was encountered while handling the remote login.", StringComparison.Ordinal))
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
