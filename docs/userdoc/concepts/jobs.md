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
Schedule defines when the job will run.  

**@startup**: Executes a job after OpenMod and all plugins have loaded, including reloads.  
**@reboot**: Executes a job after OpenMod and all plugins have loaded, excluding reloads.  
**@single_exec**: Executes a job a single time and then removes it from the file.  
**crontab expression**: Executes the job periodically, like every minute, every Sunday, every third Monday, etc. Visit https://crontab.guru/ for more information.  

Here are some example crontab expressions:  
- **0 0 \* \* 0**: Every Sunday at 0:00 AM.  
- **0 3 \* \* \***: Everyday at 3 AM.   
- **\*/5 \* \* \* \***: Every 5 minutes.  
- **0 0 1 \* \***: At the first day of every month at 0:00 AM.

## Task
The task to execute. There are 2 built-in tasks:
- **openmod_command**: Executes one or more OpenMod commands. Needs args.commands to be defined. 
- **system_command**: Executes one or more system commands. Needs args.commands to be defined. 
  
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
