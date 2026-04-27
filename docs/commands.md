# Commands

Command reference for the `nicastack` CLI.

## `nicastack`

Without arguments, it opens interactive mode.

```bash
nicastack
```

Equivalent to:

```bash
nicastack wizard
```

## `nicastack help`

Show general CLI help.

```bash
nicastack help
```

## `nicastack list`

List available templates.

```bash
nicastack list
```

## `nicastack create <name> --template <id>`

Generate a project in non-interactive mode.

### Syntax

```bash
nicastack create <name> --template <id> [options]
```

### Arguments

- `<name>`: project or module name to generate

### Options

- `-t`, `--template <id>`: template to use
- `-o`, `--output <path>`: base directory where the project will be created
- `--git-init`: initialize git when finished
- `--force`: delete the target folder if it already exists
- `--dry-run`: show the plan without creating files
- `--skip-install`: skip `dotnet new install` for NuGet templates
- `--verbose`: show details of executed processes

### Examples

```bash
nicastack create Billing --template ardalis-full
nicastack create Billing --template ardalis-minimal --output ./sandbox
nicastack create Billing --template vertical-slice-github --git-init
nicastack create Billing --template ardalis-full --dry-run
```

## `nicastack wizard`

Open the interactive wizard step by step.

### Options

- `-o`, `--output <path>`: base directory where the project will be created

### Example

```bash
nicastack wizard --output ./sandbox
```

## `nicastack doctor`

Check the local environment.

Currently checks:

- `dotnet SDK`
- `git`
- permisos de escritura en el directorio actual

```bash
nicastack doctor
```

## `nicastack version`

Show the CLI version, runtime, and operating system.

```bash
nicastack version
```

## Exit Codes

- `0`: successful operation
- `1`: execution or validation error
