# Logging
OpenMod uses [Serilog](https://serilog.net/) as the default logger implementation.

Serilog is configured through the `logging.yaml` in the OpenMod folder.
You can find the documentation for the Serilog configuration [here](https://github.com/serilog/serilog-settings-configuration) (you will have to [convert it](https://www.json2yaml.com/) from JSON to YAML).

Serilog supports many [sinks](https://github.com/serilog/serilog/wiki/Provided-Sinks) as logging target. You can install new sinks via the `openmod install <package id>` command and configure them in the logging.yaml.

For example, this is how you would configure logging to MySQL/MariaDB instead to file:

1. Install the MariaDB Sink: `openmod install Serilog.Sinks.MariaDB`
2. Add the MariaDB Sink to the logging.yaml:
```yaml
Serilog:
  Using:
  - Serilog
  - Serilog.Sinks.Console
  - Serilog.Sinks.File
  - Serilog.Sinks.Async
  - Serilog.Sinks.MariaDB # Do not forget to add your sink here
  WriteTo:
  - Name: Async # By default OpenMod logs async for performance reasons
    Args:
      configure:
      - Name: MariaDB
        Args:
          connectionString: "server=..." # See https://www.connectionstrings.com/mysql/
          tableName: "logs"
          autoCreateTable: true
      - Name: Console
        Args:
          theme: "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console"
          outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
  Enrich:
  - FromLogContext
  MinimumLevel:
    Default: Information # Only log Information or higher by default
    Override:
      # You can configure when specific events should be logged
      # In this example, the Microsoft.EntityFrameworkCore events below are only logged if Warning or higher
      Microsoft.EntityFrameworkCore.Database.Command: Warning
      Microsoft.EntityFrameworkCore.Infrastructure: Warning
      Microsoft.EntityFrameworkCore.Database.Connection: Warning
      Microsoft.EntityFrameworkCore.Query: Warning
```
