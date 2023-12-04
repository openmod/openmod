# OpenMod Jobs
Jobs are periodic tasks that execute OpenMod or system commands.  
The jobs are defined in the autoexec.yaml file.

Let's have a look at a job:
```yaml
jobs:
  # Updates OpenMod every Sunday at 0:00 AM and then restarts.
  - name: OpenMod Auto Update
    schedule: '0 0 * * 0'
    task: openmod_command
    args:
      commands:
      - openmod upgrade
      - restart
    enabled: true  
```

Let's break this down.

## Name
Sets the name of the job. Must be unique.

## Schedule
Schedule defines when a job will run. There are several ways to do it.

### One-time schedule
A one-time scheduling allows to define the conditions under which a job will be executed.

Below are the `schedule` types that define required conditions.

| **Type**     | **Description**                                                                      |
|--------------|------------------------------------------------------------------------------------- |
| @startup     | Executes a job after OpenMod and all plugins have loaded, including reloads.         |
| @reboot      | Executes a job after OpenMod and all plugins have loaded, excluding reloads.         |
| @single_exec | Executes a job a single time after OpenMod loaded and then removes it from the file. |
  
You can also delay execution of one-time scheduled job. Delayed job `schedule` template should look as further: `<schedule type>:<delay>`. 

Here are some examples of delayed job `schedule`.

| **Example schedule template**               | **Description**                                                                                                              |
|---------------------------------------------|----------------------------------------------------------------------------------------------------------------------------- |
| @startup:20 seconds                         | Executes a job 20 seconds after OpenMod and all plugins have loaded, including reloads.                                      |
| @reboot:30 days, 40 minutes, and 50 seconds | Executes a job 30 days, 40 minutes and 50 seconds after OpenMod and all plugins have loaded, excluding reloads.              |
| @single_exec:10h20m30s                      | Executes a job a single time 10 hours, 20 minutes and 30 seconds after OpenMod has loaded and then removes it from the file. |
| @startup:3.5 days                           | Executes a job 3 days and 12 hours after OpenMod and all plugins have loaded, including reloads.                             |
| @single_exec:1234.123     milliseconds      | Executes a job a single time 1234.123 milliseconds after OpenMod has loaded and then removes it from the file.               |
  
### Periodical schedule
For periodical scheduling OpenMod uses **crontab expressions**. It allows to execute a job at fixed intervals, like every minute, every Sunday, every third Monday, etc. Visit https://crontab.guru/ for more information. 

Here are some example crontab expressions.

| **Expression** | **Description**                             |
|----------------|---------------------------------------------|
| 0 0 * * 0      | Every Sunday at 0:00 AM.                    |
| 0 3 * * *      | Everyday at 3 AM.                           |
| */5 * * * *    | Every 5 minutes.                            |
| 0 0 1 * *      | At the first day of every month at 0:00 AM. |

## Task
The task to execute. There are 2 built-in tasks.

| **Task type**   | **Description**                                                           |
|-----------------|---------------------------------------------------------------------------|
| openmod_command | Executes one or more OpenMod commands. Needs args.commands to be defined. |
| system_command  | Executes one or more system commands. Needs args.commands to be defined.  |
  
Plugins can add more task types.

## Enabled
Sets if the job is enabled.

## Examples

Restart everyday at 3 AM:
```yaml
- name: Auto Restart
  schedule: '0 3 * * *'
  task: openmod_command
  args:
    commands:
    - restart
  enabled: true
```

Save every 5 minutes:
```yaml
- name: Save
  schedule: '*/5 * * * *'
  task: openmod_command
  args:
    commands:
    - save
  enabled: true
```

Run wipe.sh on every first day of the month.  It works like terminal (Linux) or cmd (Windows), so you can use scripts like .sh or .bat or run programs directly.
```yaml
- name: Wipe
  schedule: '0 0 1 * *'
  task: system_command
  args:
    commands: 
    - ./wipe.sh
  enabled: true
```

Migrate economy on startup (will only execute a single time)
```yaml
- name: Economy Migration
  schedule: '@single_exec'
  task: openmod_command
  args:
    commands: 
    - economy migrate
  enabled: true
```

Purge packages on every startup (this is not recommended and just an example).
```yaml
- name: Package purge
  schedule: '@startup'
  task: openmod_command
  args:
    commands:
    - openmod purge
  enabled: true
```

Shut the application down 5 minutes and 2 seconds after OpenMod loaded (will only execute a single time)
```yaml
- name: Initial shutdown
  schedule: '@single_exec:5m2s'
  task: openmod_command
  args:
    commands: 
    - shutdown
  enabled: true
```
