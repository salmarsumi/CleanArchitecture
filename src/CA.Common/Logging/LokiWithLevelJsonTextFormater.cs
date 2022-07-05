using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Parsing;
using Serilog.Sinks.Grafana.Loki;

namespace CA.Common.Logging
{
    public class LokiWithLevelJsonTextFormatter : ITextFormatter, ILabelAwareTextFormatter
    {
        private readonly JsonValueFormatter _valueFormatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="LokiJsonTextFormatter"/> class.
        /// </summary>
        public LokiWithLevelJsonTextFormatter()
        {
            _valueFormatter = new JsonValueFormatter("$type");
        }

        /// <inheritdoc/>
        public bool ExcludeLevelLabel => false;

        /// <summary>
        /// Format the log event into the output.
        /// </summary>
        /// <param name="logEvent">
        /// The event to format.
        /// </param>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <param name="labels">
        /// List of labels that should not be written as json fields.
        /// </param>
        public void Format(LogEvent logEvent, TextWriter output, IEnumerable<string> labels)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            output.Write("{\"Message\":");
            JsonValueFormatter.WriteQuotedJsonString(logEvent.MessageTemplate.Render(logEvent.Properties), output);

            output.Write(",\"MessageTemplate\":");
            JsonValueFormatter.WriteQuotedJsonString(logEvent.MessageTemplate.Text, output);

            var tokensWithFormat = logEvent.MessageTemplate.Tokens
                .OfType<PropertyToken>()
                .Where(pt => pt.Format != null);

            // Better not to allocate an array in the 99.9% of cases where this is false
            if (tokensWithFormat.Any())
            {
                output.Write(",\"Renderings\":[");
                var delimiter = string.Empty;
                foreach (var r in tokensWithFormat)
                {
                    output.Write(delimiter);
                    delimiter = ",";
                    var space = new StringWriter();
                    r.Render(logEvent.Properties, space);
                    JsonValueFormatter.WriteQuotedJsonString(space.ToString(), output);
                }

                output.Write(']');
            }

            output.Write(",\"level\":\"");
            output.Write(logEvent.Level.ToGrafanaLogLevel());
            output.Write('\"');

            if (logEvent.Exception != null)
            {
                output.Write(",\"Exception\":");
                SerializeException(output, logEvent.Exception, 1);
            }

            foreach (var property in logEvent.Properties)
            {
                var name = property.Key;
                if (labels.Contains(name))
                {
                    continue;
                }

                output.Write(',');
                JsonValueFormatter.WriteQuotedJsonString(name, output);
                output.Write(':');
                _valueFormatter.Format(property.Value, output);
            }

            output.Write('}');
        }

        /// <inheritdoc/>
        [Obsolete("Use \"Format(LogEvent logEvent, TextWriter output, IEnumerable<string> labels)\" instead!")]
        public void Format(LogEvent logEvent, TextWriter output) => Format(logEvent, output, Enumerable.Empty<string>());

        /// <summary>
        /// Used to serialize exceptions, can be overridden when inheriting to change the format.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <param name="exception">
        /// The exception to format.
        /// </param>
        /// <param name="level">
        /// The current nesting level of the exception.
        /// </param>
        protected virtual void SerializeException(TextWriter output, Exception exception, int level)
        {
            if (level == 4)
            {
                JsonValueFormatter.WriteQuotedJsonString(exception.ToString(), output);
                return;
            }

            output.Write("{\"Type\":");
            var typeNamespace = exception.GetType().Namespace;
            var typeName = typeNamespace != null && typeNamespace.StartsWith("System.") ? exception.GetType().Name : exception.GetType().ToString();
            JsonValueFormatter.WriteQuotedJsonString(typeName, output);

            if (!string.IsNullOrWhiteSpace(exception.Message))
            {
                output.Write(",\"Message\":");
                JsonValueFormatter.WriteQuotedJsonString(exception.Message, output);
            }

            if (!string.IsNullOrWhiteSpace(exception.StackTrace))
            {
                output.Write(",\"StackTrace\":");
                JsonValueFormatter.WriteQuotedJsonString(exception.StackTrace, output);
            }

            if (exception is AggregateException aggregateException)
            {
                output.Write(",\"InnerExceptions\":[");
                var count = aggregateException.InnerExceptions.Count;
                for (var i = 0; i < count; i++)
                {
                    var isLast = i == count - 1;
                    SerializeException(output, aggregateException.InnerExceptions[i], level + 1);
                    if (!isLast)
                    {
                        output.Write(',');
                    }
                }

                output.Write("]");
            }
            else if (exception.InnerException != null)
            {
                output.Write(",\"InnerException\":");
                SerializeException(output, exception.InnerException, level + 1);
            }

            output.Write('}');
        }
    }

    internal static class LogEventLevelExtensions
    {
        internal static string ToGrafanaLogLevel(this LogEventLevel level) =>
            level switch
            {
                LogEventLevel.Verbose => "trace",
                LogEventLevel.Debug => "debug",
                LogEventLevel.Information => "info",
                LogEventLevel.Warning => "warning",
                LogEventLevel.Error => "error",
                LogEventLevel.Fatal => "critical",
                _ => "unknown"
            };
    }
}
