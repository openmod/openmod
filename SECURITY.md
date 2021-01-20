# Security Policy

## Supported Versions
Only the latest major version is supported. Other versions will not receive security updates.

| Version | Supported          |
| ------- | ------------------ |
| 3.0.x   | :white_check_mark: |
| 2.x.x   | :x:                |
| 1.x.x   | :x:                |
| < 1.0   | :x:                |

## Reporting an OpenMod Vulnerability

To report a security vulnerability please join [our Discord](https://discord.com/invite/jRrCJVm) and contact one of the maintainers. Do not disclose security issues publicly.  
Please do not report security issues that are caused by 3rd party plugins.

Critical vulnerabilities include:
- Remote code execution (e.g. via commands, specially crafted network packets, deserialization attacks, etc.)
- Privilege escalation (acquiring access to restricted commands and other actions)
- SQL injections
- Arbitrary disk I/O as a user (e.g. reading or writing files with built-in or official plugin commands)
- etc.

## Reporting Malicious Plugins
Please report malicious NuGet packages to NuGet by clicking "Report" on the right sidebar after opening the package's page. If the plugin is not hosted on NuGet, please contact the platform hosting it.
