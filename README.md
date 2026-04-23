Requisitos
Tener instalado .NET 8 SDK.
Tener acceso a una instancia de SQL Server.
Configurar la cadena de conexión en appsettings.json en ConnectionStrings:SqlServer

Tecnologías usadas 
ASP.NET Core Web API (.NET 8): framework sólido para construir APIs REST mantenibles.
EF Core + SQL Server: persistencia real con LINQ, mapeo fuerte y buena integración con .NET.
Arquitectura por capas (Api, Application, Domain, Infrastructure): separa responsabilidades y hace escalable la solución.
JWT Bearer: autenticación simple y estándar para proteger endpoints.
Swagger / OpenAPI: facilita pruebas manuales y documentación.
OpenAI + mock fallback: permite cumplir integración con IA sin depender obligatoriamente de una API key.

Cómo ejecutar el seed
No se necesita un comando aparte. El seed corre automáticamente al arrancar la API si la tabla no tiene registros.
El inicializador está en:
ApplicationDbInitializer.cs

Login:
POST /auth/login

{
  "username": "admin",
  "password": "123*"
}

Aunque el proyecto usa appsettings.json, este sería el contenido equivalente de referencia, sin valores reales:
CONNECTIONSTRINGS__SQLSERVER=Server=YOUR_SERVER;Database=YOUR_DB;Trusted_Connection=True;TrustServerCertificate=True;
.env.example
OPENAI__APIKEY=
OPENAI__BASEURL=https://api.openai.com/v1/
OPENAI__MODEL=gpt-5

JWT__ISSUER=OneMillionCopy.Leads.Api
JWT__AUDIENCE=OneMillionCopy.Leads.Client
JWT__SECRETKEY=YOUR_STRONG_SECRET_KEY
JWT__EXPIRATIONMINUTES=60

AUTH__USERNAME=admin
AUTH__PASSWORD=YOUR_ADMIN_PASSWORD


Manejo de errores
La API tiene manejo consistente mediante middleware global en:
ExceptionHandlingMiddleware.cs

Comportamiento:
400 para validaciones
404 para recursos no encontrados
409 para conflictos como email duplicado
500 para errores no controlados

