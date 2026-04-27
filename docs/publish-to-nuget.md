# Publish to NuGet

Esta guia cubre el flujo para publicar `nicastack` como .NET tool en NuGet y automatizar el release.

## Requisitos

- una cuenta en [nuget.org](https://www.nuget.org/)
- ownership del package id `NicaStack.Cli`
- un API key de NuGet con permiso de `Push`
- el repositorio hospedado en GitHub

## Publicacion manual

Desde la raiz del repo:

```bash
dotnet pack NicaStack.Cli/NicaStack.Cli.csproj -c Release
dotnet nuget push NicaStack.Cli/nupkg/*.nupkg --api-key <TU_API_KEY> --source https://api.nuget.org/v3/index.json
```

## Automatizacion con GitHub Actions

El workflow incluido publica el paquete cuando haces push de un tag con formato `v*.*.*`.

Ejemplo:

```bash
git tag v0.1.0
git push origin v0.1.0
```

El workflow hace esto:

1. restaura dependencias
2. compila el proyecto
3. empaqueta el tool con la version del tag
4. publica el `.nupkg` en nuget.org

## Secret requerido

Configura este secret en GitHub:

- `NUGET_API_KEY`

Ruta en GitHub:

`Settings > Secrets and variables > Actions > New repository secret`

## Formato de version

El workflow toma la version desde el tag.

- `v0.1.0` publica version `0.1.0`
- `v1.2.3` publica version `1.2.3`

## Instalar el tool desde NuGet

Una vez publicado:

```bash
dotnet tool install --global NicaStack.Cli
```

Actualizar:

```bash
dotnet tool update --global NicaStack.Cli
```

## Notas

- si el repositorio aun no esta en GitHub, el workflow no correra todavia
- si el package id ya existe y no eres owner, NuGet rechazara la publicacion
- si quieres metadata mas completa, agrega `RepositoryUrl`, `PackageProjectUrl` e icono cuando el repo ya exista