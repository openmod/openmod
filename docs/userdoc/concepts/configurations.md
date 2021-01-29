# Configuration files

OpenMod uses [YAML](https://yaml.org/) for configurations.  
You can find a quick YAML guide [here](https://blog.codemagic.io/what-you-can-do-with-yaml/).

Plugin specific configurations are stored in the `plugins/<plugin id>` directory.

> [!WARNING]
> Configuration files are currently not updated automatically. If OpenMod or a plugin adds a new field, you will have to add it manually to the yaml file or delete the file so it gets recreated.