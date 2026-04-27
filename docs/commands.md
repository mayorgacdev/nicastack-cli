# Commands

Referencia de comandos del CLI `nicastack`.

## `nicastack`

Sin argumentos, abre el modo interactivo.

```bash
nicastack
```

Equivale a:

```bash
nicastack wizard
```

## `nicastack help`

Muestra la ayuda general del CLI.

```bash
nicastack help
```

## `nicastack list`

Lista los templates disponibles.

```bash
nicastack list
```

## `nicastack create <name> --template <id>`

Genera un proyecto en modo no interactivo.

### Sintaxis

```bash
nicastack create <name> --template <id> [options]
```

### Argumentos

- `<name>`: nombre del proyecto o modulo a generar

### Opciones

- `-t`, `--template <id>`: template a usar
- `-o`, `--output <path>`: directorio base donde se creara el proyecto
- `--git-init`: inicializa git al finalizar
- `--force`: borra la carpeta destino si ya existe
- `--dry-run`: muestra el plan sin crear archivos
- `--skip-install`: omite `dotnet new install` para templates NuGet
- `--verbose`: muestra detalle de los procesos ejecutados

### Ejemplos

```bash
nicastack create Billing --template ardalis-full
nicastack create Billing --template ardalis-minimal --output ./sandbox
nicastack create Billing --template vertical-slice-github --git-init
nicastack create Billing --template ardalis-full --dry-run
```

## `nicastack wizard`

Abre el asistente interactivo paso a paso.

### Opciones

- `-o`, `--output <path>`: directorio base donde se creara el proyecto

### Ejemplo

```bash
nicastack wizard --output ./sandbox
```

## `nicastack doctor`

Verifica el entorno local.

Actualmente revisa:

- `dotnet SDK`
- `git`
- permisos de escritura en el directorio actual

```bash
nicastack doctor
```

## `nicastack version`

Muestra la version del CLI, runtime y sistema operativo.

```bash
nicastack version
```

## Codigos de salida

- `0`: operacion exitosa
- `1`: error de ejecucion o validacion
