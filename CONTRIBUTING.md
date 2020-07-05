# Contributing to OpenMod
First off, thanks for taking your time to contribute 👍

Please read this file before opening a pull request.

## Before Starting 
Before starting writing code, there are some things you should do first.

Unless you are planning to do minor changes, you should open an issue first. Discuss your planned changes with the maintainers to avoid investing time and getting your pull request rejected.

## Do's and Don'ts
**Do**
- Write simple and readable code. The goal isn't to write code as short as possible.
- Apply C# and Unity best practices.
- Use design patterns.
- Use Resharper to conform to our code and naming conventions.
- Consider discussing your planned changes in issues before writing code.
- Write XMLDoc documentation (`/// \<summary\>`) for interfaces. 
- Write comments when neccessary, but do not comment the obvious. Avoid too many comments.

**Don't**
- Never do "cleanup" commits which format whole projects and change a lot of files.
- Avoid breaking changes, especially if it breaks plugins.
- Avoid committing code that does not compile.
- Do not use placeholder commit messages such as "..". Instead, describe what your commit does.

## Testing the Changes
After writing your code, you can test your changes by launching the OpenMod.Standalone project. Ensure you did not break anything before opening a pull request.

## Coding and Naming Conventions
We use Resharper to enforce our coding and naming conventions. If you do not use use Resharper, here is a reference for them:

### Coding Convention
**I. Types**
- a. Types must be named PascalCase: EventBus, CommandStore, etc.
- b. Interfaces must be prefixed with an "I": IPlayer, ICommandContextAccessor, etc.
- c. Enums are not prefixed or suffixed: PermissionGrantResult.
- d. Structs are not prefixed or suffixed: RuntimeStartParameters.
- e. Each file may only contain one type unless using nested classes.
- f. The file name must match the type name: EventBus.cs, CommandStore.cs, etc.

**II. Fields**
- a. Private fields are prefixed with m_ and written in PascalCase: m_CommandSource.
- b. Private static fields are prefixed with s_ and written in PascalCase: s_Lock.
- c. Private const fields are prefixed with c_ and written in PascalCase: c_HarmonyId.
- d. Protected and Public fields are not allowed unless it's constant. Must use properties otherwise.

**III. Properties**
- a. Properties must be named PascalCase: Player, Context, etc.
- b. Do not use the "=>" shortcut for getter or setter bodies.

**IV. Methods**
- a. Methods must be named PascalCase: GetPlayers(), BuildContext(), etc. 
- b. Parameters and local variables must be named camelCase: player, command, commandProvider etc.
- c. Methods which return Tasks must be suffixed with Async: GetPlayersAsync(), Task BuildContextAsync(), etc.
- d. Do not use the "=>" shortcut for method bodies.
- e. Avoid getter/setter methods which can be designed as properties instead: use the Player property instead of the GetPlayer() method.

**V. Misc**
- a. Usage of the `global::` alias is forbidden.
- b. Always use brackets, even for one liner if's, for, foreach, while etc.