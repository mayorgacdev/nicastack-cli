# Get Started

Esta guia te lleva desde cero hasta generar tu primer proyecto con `nicastack`.

## Antes de empezar

Necesitas:

- .NET SDK instalado
- `git` instalado
- permisos de escritura en el directorio de trabajo

Valida eso con:

```bash
nicastack doctor
```

## Opcion 1: modo guiado

Si no quieres recordar flags ni templates, ejecuta:

```bash
nicastack
```

O de forma explicita:

```bash
nicastack wizard
```

El wizard hace esto:

1. Te muestra los templates disponibles.
2. Te pide el nombre del proyecto o modulo.
3. Te pregunta si quieres inicializar git.
4. Genera el proyecto y te muestra el siguiente paso.

## Opcion 2: modo directo

Si ya sabes el template que quieres usar:

```bash
nicastack create Billing --template ardalis-full
```

Ejemplo con git:

```bash
nicastack create Billing --template vertical-slice-github --git-init
```

## Revisar templates

Para listar las opciones disponibles:

```bash
nicastack list
```

## Probar sin crear archivos

Para revisar el plan antes de ejecutar:

```bash
nicastack create Billing --template ardalis-full --dry-run
```

## Siguiente paso recomendado

Despues de generar el proyecto:

```bash
cd Billing
```
