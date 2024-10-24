# Bank API Service

Este proyecto es un servicio API que simula transacciones bancarias y está dividido en dos servicios principales:

- **Clientes y Personas**: Gestión de clientes y personas vinculadas a sus cuentas bancarias.
- **Movimientos y Cuentas**: Gestión de cuentas bancarias y sus movimientos asociados (depósitos, retiros, etc.).

## Arquitectura

El proyecto sigue una **arquitectura en capas** que separa las responsabilidades en diferentes módulos, mejorando la mantenibilidad y escalabilidad de la aplicación. Estas capas son:

1. **Capa de Aplicación**: Contiene los servicios que manejan la lógica de negocio de clientes, cuentas y movimientos.
2. **Capa de Infraestructura**: Maneja la interacción con las bases de datos y otros sistemas externos.
3. **Capa de Presentación (API)**: Expone los endpoints a los clientes (por ejemplo, mediante Swagger o Postman).

## Comunicación entre servicios

Los servicios de **ClientesPersonas** y **CuentasMovimientos** están **separados** y se comunican **mediante HTTP de forma asíncrona**. Esto permite que cada servicio sea autónomo y pueda ser escalado de forma independiente. Por ejemplo, el servicio de **CuentasMovimientos** consulta los detalles de un cliente mediante peticiones HTTP al servicio de **ClientesPersonas**.

### Tecnologías utilizadas

- **.NET 7**: Framework principal para el desarrollo de la API.
- **Entity Framework Core**: Para la interacción con la base de datos.
- **SQL Server**: Base de datos utilizada, desplegada en Docker para su integración.
- **Swagger**: Para la documentación y prueba de la API.

## Configuración de la base de datos

Este proyecto está configurado para trabajar con **SQL Server** o una **base de datos en memoria**. En el archivo de configuración (`appsettings.json` o `appsettings.docker.json`), se puede especificar cuál de estas opciones usar.

Para habilitar la **base de datos en memoria**, debe configurarse el valor `BaseDeDatosEnMemoria` en el archivo de configuración:

```json
{
  "BaseDeDatosEnMemoria": true
}
```

Si se desea utilizar **SQL Server**, el valor debe ser `false`, y en la sección `ConnectionStrings` se debe definir la cadena de conexión correcta al servidor SQL:

```json
{
  "BaseDeDatosEnMemoria": false,
  "ConnectionStrings": {
    "DefaultConnection": "Server=sqlserver,1433;Database=DataBank;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;"
  }
}
```

Esto permite flexibilidad para entornos de desarrollo o pruebas donde no se tenga acceso a un servidor SQL.

## Endpoints principales

### Servicio ClientesPersonas

- `GET /api/Cliente/{id}`: Obtiene los detalles de un cliente.
- `POST /api/Cliente`: Crea un nuevo cliente.

### Servicio CuentasMovimientos

- `POST /api/Cuenta`: Crea una nueva cuenta bancaria.
- `GET /api/Cuenta/{numeroCuenta}`: Obtiene los detalles de una cuenta.
- `DELETE /api/Cuenta/{numeroCuenta}`: Elimina una cuenta solo si no tiene movimientos asociados.
- `POST /api/Movimiento`: Registra un movimiento (depósito/retiro) en una cuenta bancaria.

El procesamiento de movimientos se realiza de manera automática. Según los datos enviados, internamente se determina si es un depósito o un retiro.

### Ejemplo de uso

#### Registro de un nuevo movimiento
```json
{
  "numeroCuenta": 12344,
  "tipo": "Ahorros",
  "saldoInicial": 110,
  "estado": true,
  "movimiento": "Retiro de 10"
}
```
El sistema procesa el movimiento y lo registra correctamente, actualizando el saldo de la cuenta.

## Requisitos

- **Docker**: Para la ejecución del SQL Server en un contenedor.
- **Visual Studio 2022**: Recomiendo ejecutar el proyecto directamente en Visual Studio si estás en un entorno Mac, ya que la integración con SQL Server en Docker puede presentar algunos inconvenientes.
- **Postman**: Los archivos JSON para las pruebas en Postman están incluidos en el proyecto.

## Configuración

La configuración para el despliegue en Docker y la base de datos SQL Server está en los archivos `.yml`. Actualmente está configurado para trabajar en una MacBook Pro, por lo que puede ser necesario ajustarlo para otros entornos.

## Ejecución

Los servicios se encuentran expuestos en las siguientes URLs:

- **ClientesPersonas**: `https://localhost:5001/`
- **CuentasMovimientos**: `https://localhost:6001/`

Los archivos de despliegue en Docker, junto con la base de datos y los archivos JSON de Postman, están incluidos en el repositorio.
