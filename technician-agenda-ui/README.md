# 🔧 Agenda de Técnicos

Sistema completo de gestión de órdenes de trabajo para técnicos con backend en .NET 8 y frontend en React.

![Version](https://img.shields.io/badge/version-1.0.0-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![React](https://img.shields.io/badge/React-18-blue)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-red)
![Redis](https://img.shields.io/badge/Redis-7-red)

## 📋 Características

### Gestión Completa
- ✅ **Clientes**: Registro completo con múltiples direcciones
- ✅ **Técnicos**: Perfil detallado con certificaciones y vehículo
- ✅ **Órdenes de Trabajo**: Asignación, seguimiento y gestión de pagos
- ✅ **Categorías**: 17 categorías de servicios con subcategorías
- ✅ **Archivos**: Soporte para imágenes y videos
- ✅ **Pagos**: Control de pagos a clientes y técnicos

### Características Técnicas
- 🚀 **Cache Redis** para alto rendimiento
- 📱 **Responsive Design** con Tailwind CSS
- 🔒 **CORS configurado** para seguridad
- 📊 **API RESTful** documentada con Swagger
- 🗄️ **Entity Framework Core** con migraciones
- 📍 **Regiones y Comunas** de Chile precargadas

## 🛠️ Tecnologías

### Backend
- .NET 8.0
- Entity Framework Core
- SQL Server 2022
- Redis Cache
- Swagger/OpenAPI

### Frontend
- React 18
- Axios
- Tailwind CSS
- React Hooks

## 🚀 Instalación Local

### Prerrequisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- [SQL Server 2022](https://www.microsoft.com/sql-server)
- [Redis](https://redis.io/download) o [Docker](https://www.docker.com/)

### 1. Clonar repositorio
```bash
git clone https://github.com/tu-usuario/technician-agenda.git
cd technician-agenda
```

### 2. Configurar Backend

```bash
cd TechnicianAgenda

# Restaurar paquetes
dotnet restore

# Configurar connection string en appsettings.json
# Editar: "DefaultConnection": "Server=localhost;Database=TechnicianAgendaDB;..."

# Crear base de datos
dotnet ef database update

# Ejecutar
dotnet run --launch-profile https
```

La API estará disponible en: `https://localhost:7054`
Swagger UI: `https://localhost:7054/swagger`

### 3. Configurar Redis

**Opción A - Docker (Recomendado):**
```bash
docker run -d -p 6379:6379 --name technician-redis redis:7-alpine
```

**Opción B - Windows:**
- Descargar desde: https://github.com/microsoftarchive/redis/releases
- Ejecutar redis-server.exe

### 4. Configurar Frontend

```bash
cd technician-agenda-ui

# Instalar dependencias
npm install

# Ejecutar en desarrollo
npm start
```

La aplicación estará disponible en: `http://localhost:3000`

## 🐳 Docker (Alternativa Rápida)

```bash
# Levantar todo el stack
docker-compose up -d

# Ver logs
docker-compose logs -f

# Detener
docker-compose down
```

Esto levanta:
- SQL Server en puerto 1433
- Redis en puerto 6379  
- Backend API en puerto 7054
- Frontend en puerto 3000

## 📊 Estructura del Proyecto

```
technician-agenda/
├── TechnicianAgenda/              # Backend .NET
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Models/
│   │   ├── Client.cs
│   │   ├── Technician.cs
│   │   ├── Work.cs
│   │   └── ...
│   ├── Migrations/
│   ├── Program.cs
│   └── appsettings.json
│
├── technician-agenda-ui/          # Frontend React
│   ├── public/
│   ├── src/
│   │   ├── App.js
│   │   └── index.js
│   ├── package.json
│   └── tailwind.config.js
│
├── docker-compose.yml
└── README.md
```

## 🔧 Configuración

### appsettings.json (Backend)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TechnicianAgendaDB;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

### .env (Frontend - crear en raíz de technician-agenda-ui)
```env
REACT_APP_API_URL=https://localhost:7054/api
```

## 📚 API Endpoints

### Clientes
- `GET /api/clients` - Listar todos los clientes
- `POST /api/clients` - Crear cliente
- `GET /api/clients/{id}` - Obtener cliente

### Técnicos
- `GET /api/technicians` - Listar técnicos
- `POST /api/technicians` - Crear técnico
- `PUT /api/technicians/{id}` - Actualizar técnico
- `DELETE /api/technicians/{id}` - Eliminar técnico

### Trabajos
- `GET /api/works` - Listar órdenes
- `POST /api/works` - Crear orden
- `PUT /api/works/{id}` - Actualizar orden
- `PATCH /api/works/{id}/status` - Cambiar estado
- `PATCH /api/works/{id}/technician-payment` - Marcar pago a técnico

Ver documentación completa en Swagger: `https://localhost:7054/swagger`

## 🌐 Deployment

Ver guía completa de deployment en [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md)

### Deploy Rápido con Railway (Backend)
```bash
railway login
railway init
railway up
```

### Deploy Frontend con Vercel
```bash
cd technician-agenda-ui
npm run build
vercel --prod
```

## 🤝 Contribuir

1. Fork el proyecto
2. Crea tu rama (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📝 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.

## 👥 Autor

Tu Nombre - [@tu_twitter](https://twitter.com/tu_twitter)

Proyecto Link: [https://github.com/tu-usuario/technician-agenda](https://github.com/tu-usuario/technician-agenda)

## 🙏 Agradecimientos

- [.NET](https://dotnet.microsoft.com/)
- [React](https://reactjs.org/)
- [Tailwind CSS](https://tailwindcss.com/)
- [Redis](https://redis.io/)

---

⭐️ Si este proyecto te fue útil, dale una estrella en GitHub!
