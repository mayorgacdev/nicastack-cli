# Installation

`nicastack` esta empaquetado como .NET tool.

## Requisitos

- .NET SDK 10 o compatible con el target del proyecto
- `git`
- macOS, Linux o Windows con soporte para `dotnet`

## Instalar desde codigo fuente

Desde la raiz del repositorio:

```bash
dotnet build NicaStack.Cli/NicaStack.Cli.csproj
```

Empaqueta el tool:

```bash
dotnet pack NicaStack.Cli/NicaStack.Cli.csproj -c Release
```

Eso genera el paquete en:

```text
NicaStack.Cli/nupkg/
```

Instala el tool desde el paquete local:

```bash
dotnet tool install --global --add-source ./NicaStack.Cli/nupkg NicaStack.Cli
```

Si ya estaba instalado y quieres actualizarlo:

```bash
dotnet tool update --global --add-source ./NicaStack.Cli/nupkg NicaStack.Cli
```

## Verificar instalacion

```bash
nicastack help
nicastack version
nicastack doctor
```

## Ejecutarlo sin instalar

Tambien puedes usarlo directamente desde el proyecto:

```bash
dotnet run --project NicaStack.Cli -- help
```

## Desinstalar

```bash
dotnet tool uninstall --global NicaStack.Cli
```

## Publicacion futura a NuGet

Si publicas el paquete en NuGet, la instalacion seria asi:

```bash
dotnet tool install --global NicaStack.Cli
```

Ese comando aplica solo cuando el paquete ya exista en una fuente publica o privada configurada.
