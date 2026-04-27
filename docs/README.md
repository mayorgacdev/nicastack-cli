# NicaStack CLI Docs

Documentacion oficial del CLI `nicastack`.

## Contenido

- [Get Started](./get-started.md)
- [Installation](./installation.md)
- [Commands](./commands.md)
- [Templates](./templates.md)
- [Publish to NuGet](./publish-to-nuget.md)
- [Troubleshooting](./troubleshooting.md)

## Que es NicaStack CLI

`nicastack` es un .NET tool para generar proyectos base a partir de templates de arquitectura. Tiene dos formas de uso:

- Modo guiado: ejecuta `nicastack` o `nicastack wizard`
- Modo no interactivo: ejecuta `nicastack create <name> --template <id>`

## Comandos disponibles

- `help`
- `list`
- `create`
- `wizard`
- `doctor`
- `version`

## Flujo recomendado

1. Instala o compila el CLI.
2. Ejecuta `nicastack doctor` para validar el entorno.
3. Ejecuta `nicastack list` para ver templates.
4. Usa `nicastack` para modo interactivo o `nicastack create` para modo directo.

## Ejemplo rapido

```bash
nicastack list
nicastack create Billing --template ardalis-full --git-init
```
