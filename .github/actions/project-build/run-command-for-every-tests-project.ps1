#
# Finds all tests projects for a project passed as the first argument
# and runs a command passed as the second argument for every tests project found.
# Also sets PROJECT_PATH environment variables with value of the tests project folder path.
#
# Example usage:
# pwsh -f .github/actions/project-build/run-command-for-every-tests-project.ps1 "framework/OpenMod.Core" "echo \$PROJECT_PATH"
#
# Example output:
# Tests project found: framework/OpenMod.Core/../tests/OpenMod.Core.Tests. Executing a command: echo $PROJECT_PATH
# framework/OpenMod.Core/../tests/OpenMod.Core.Tests

$projectPath = $args[0]
$projectName = Split-Path -Path $projectPath -Leaf
$testsFolderPath = Join-Path -Path $projectPath -ChildPath "../tests"

$commandToExecute = $args[1]

Get-ChildItem -Path $testsFolderPath -Directory -Recurse `
| Where-Object { $_.Name -match "^$projectName.*Tests$" } `
| ForEach-Object {
    $testsProjectName = $_.Name
    $testsProjectPath = Join-Path -Path $testsFolderPath -ChildPath $testsProjectName
    Write-Output "Tests project found: $testsProjectPath. Executing a command: $commandToExecute"
    bash -c "PROJECT_PATH=$testsProjectPath && $commandToExecute"
}
