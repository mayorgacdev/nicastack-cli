# Templates

`nicastack` resuelve templates desde un catalogo interno.

## `ardalis-full`

- Nombre: Ardalis Full Clean Architecture
- Origen: NuGet
- Tipo: `DotnetTemplate`
- Paquete: `Ardalis.CleanArchitecture.Template`
- Short name: `clean-arch`
- Descripcion: solucion multi-proyecto con capas Domain, Application, Infrastructure y Web

### Ejemplo

```bash
nicastack create Billing --template ardalis-full
```

## `ardalis-minimal`

- Nombre: Ardalis Minimal Clean
- Origen: NuGet
- Tipo: `DotnetTemplate`
- Paquete: `Ardalis.MinimalClean.Template`
- Short name: `min-clean`
- Descripcion: plantilla ligera orientada a API y vertical slices

### Ejemplo

```bash
nicastack create Billing --template ardalis-minimal
```

## `vertical-slice-github`

- Nombre: Nadirbad Vertical Slice
- Origen: GitHub
- Tipo: `GitClone`
- Repositorio: `https://github.com/nadirbad/VerticalSliceArchitecture.git`
- Namespace base reemplazado: `VerticalSliceArchitecture`
- Descripcion: clona un repositorio base y reemplaza el namespace raiz

### Ejemplo

```bash
nicastack create Billing --template vertical-slice-github --git-init
```

## Como elegir un template

Usa `ardalis-full` si quieres una base mas clasica y multi-proyecto.

Usa `ardalis-minimal` si quieres una base mas ligera y rapida para API.

Usa `vertical-slice-github` si quieres arrancar desde una implementacion basada en vertical slice y clonado de repositorio.
