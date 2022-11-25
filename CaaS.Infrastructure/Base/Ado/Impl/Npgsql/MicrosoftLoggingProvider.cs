using Microsoft.Extensions.Logging;
using Npgsql.Logging;

namespace CaaS.Infrastructure.Base.Ado.Impl.Npgsql; 

public class MicrosoftLoggingProvider : INpgsqlLoggingProvider {
    private readonly ILoggerFactory _loggerFactory;

    public MicrosoftLoggingProvider(ILoggerFactory loggerFactory) {
        _loggerFactory = loggerFactory;
    }

    public NpgsqlLogger CreateLogger(string name) {
        return new MicrosoftLoggingLogger(_loggerFactory.CreateLogger(name));
    }
}

internal class MicrosoftLoggingLogger : NpgsqlLogger {
    private readonly ILogger _logger;

    public MicrosoftLoggingLogger(ILogger logger) {
        _logger = logger;
    }

    public override bool IsEnabled(NpgsqlLogLevel level) {
        return _logger.IsEnabled(ToMicrosoftLogLevel(level));
    }

    public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception? exception = null) {
        // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
#pragma warning disable CA2254
        _logger.Log(ToMicrosoftLogLevel(level), exception, msg, connectorId);
#pragma warning restore CA2254
    }

    private static LogLevel ToMicrosoftLogLevel(NpgsqlLogLevel level) {
        return level switch {
            NpgsqlLogLevel.Trace => LogLevel.Trace,
            NpgsqlLogLevel.Debug => LogLevel.Debug,
            NpgsqlLogLevel.Info => LogLevel.Information,
            NpgsqlLogLevel.Warn => LogLevel.Warning,
            NpgsqlLogLevel.Error => LogLevel.Error,
            NpgsqlLogLevel.Fatal => LogLevel.Critical,
            _ => throw new ArgumentOutOfRangeException(nameof(level))
        };
    }
}