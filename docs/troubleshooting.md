# Troubleshooting

Problemas comunes al usar `nicastack`.

## `help` no funciona si usas `dotnet run`

Usa esta forma:

```bash
dotnet run --project NicaStack.Cli -- help
```

El `--` separa los argumentos de `dotnet run` de los argumentos del CLI.

## `Debes indicar --template`

El comando `create` requiere que indiques el template.

Ejemplo correcto:

```bash
nicastack create Billing --template ardalis-full
```

## La carpeta destino ya existe

Si el directorio del proyecto ya existe, puedes:

1. cambiar el nombre del proyecto
2. usar otro `--output`
3. usar `--force` para borrar el destino y volver a generarlo

Ejemplo:

```bash
nicastack create Billing --template ardalis-full --force
```

## Falla `dotnet new install`

Eso puede pasar por:

- conectividad de red
- nombre de paquete invalido
- problema temporal con la fuente de paquetes

Prueba:

```bash
nicastack doctor
nicastack create Billing --template ardalis-full --verbose
```

## Falla `git`

Algunos templates requieren `git`. Verifica instalacion:

```bash
git --version
nicastack doctor
```

## Quiero ver el plan antes de ejecutar

Usa `--dry-run`:

```bash
nicastack create Billing --template ardalis-full --dry-run
```

## Quiero una experiencia guiada

Usa:

```bash
nicastack
```

O:

```bash
nicastack wizard
```
