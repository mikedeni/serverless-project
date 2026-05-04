# MyBrick - Construction SaaS Platform

MyBrick is a comprehensive Software-as-a-Service (SaaS) platform for managing construction projects, built with modern technologies including C# .NET backend, React frontend, MySQL database, and Docker containerization.

## 📋 Prerequisites

### Common Requirements
- **Docker** (version 20.10+) and **Docker Compose** (version 1.29+)
- **Git**

### For Local Development (without Docker)
- **.NET 6.0 or higher** (for backend)
- **Node.js** (16+ or 18+) and **npm** (for frontend)
- **MySQL Server** (8.0+)

## 🚀 Quick Start with Docker

The recommended way to run the entire project is using Docker Compose.

### 1. Build and Start All Services

```bash
cd /home/peter/Desktop/MyBrick/MyBrick
docker-compose up --build
```

This will start:
- **MySQL Database** (port 3306)
- **Backend API** (port 5154)
- **Frontend Application** (port 80)
- **Prometheus Monitoring** (port 9090)

### 2. Access the Applications

- **Frontend**: http://localhost
- **Backend API**: http://localhost:5154
- **Swagger API Docs**: http://localhost:5154/swagger
- **Prometheus**: http://localhost:9090

### 3. Stop Services

```bash
docker-compose down
```

To also remove persistent database data:
```bash
docker-compose down -v
```

## 🔧 Local Development Setup

### Backend Setup

1. **Navigate to backend directory**
   ```bash
   cd backend
   ```

2. **Configure Database Connection**
   
   Update `appsettings.json` with your local database credentials:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=ConstructionSaaS;User=mybrick_user;Password=MyBrick@2026;"
     }
   }
   ```

3. **Install Dependencies and Run**
   ```bash
   dotnet restore
   dotnet run
   ```

   The backend will start on `http://localhost:5154`

### Frontend Setup

1. **Navigate to frontend directory**
   ```bash
   cd frontend
   ```

2. **Install Dependencies**
   ```bash
   npm install
   ```

3. **Start Development Server**
   ```bash
   npm run dev
   ```

   The frontend will start on `http://localhost:5173` (Vite default)

### Database Setup (Local)

1. **Create Database**
   ```sql
   CREATE DATABASE ConstructionSaaS;
   ```

2. **Create User**
   ```sql
   CREATE USER 'mybrick_user'@'localhost' IDENTIFIED BY 'MyBrick@2026';
   GRANT ALL PRIVILEGES ON ConstructionSaaS.* TO 'mybrick_user'@'localhost';
   FLUSH PRIVILEGES;
   ```

3. **Run Schema**
   ```bash
   mysql -u mybrick_user -p ConstructionSaaS < database/schema.sql
   ```

## 📁 Project Structure

```
MyBrick/
├── backend/                 # .NET 6 C# Backend
│   ├── Controllers/         # API endpoints
│   ├── Services/            # Business logic
│   ├── Repositories/        # Data access layer
│   ├── Models/              # Database models
│   ├── DTOs/                # Data transfer objects
│   ├── Program.cs           # Application entry point
│   └── appsettings.json     # Configuration
├── frontend/                # React + Vite Frontend
│   ├── src/                 # React components and pages
│   ├── public/              # Static assets
│   └── vite.config.js       # Vite configuration
├── database/                # Database schemas
│   ├── schema.sql           # Initial database schema
│   └── update.sql           # Database migrations
├── docker-compose.yml       # Docker services configuration
└── prometheus.yml           # Prometheus monitoring config
```

## 🐳 Docker Services

### MySQL Database
- **Image**: mysql:8.0
- **Container**: mybrick-db
- **Port**: 3306
- **Database**: ConstructionSaaS
- **Credentials**: 
  - Username: `mybrick_user`
  - Password: `MyBrick@2026`

### Backend API
- **Port**: 5154
- **Environment Variables**:
  - `ConnectionStrings__DefaultConnection`: MySQL connection string
  - `Jwt__Key`: JWT secret key

### Frontend
- **Port**: 80
- **Framework**: React 18 with Vite

### Prometheus
- **Port**: 9090
- **Purpose**: Metrics collection and monitoring

## 🔐 Default Configuration

### JWT Configuration
- **Issuer**: MyBrick
- **Audience**: MyBrickUsers
- **Key**: super_secret_jwt_key_for_mybrick_construction_platform_2026

**⚠️ Important**: Change the JWT key in production!

## 📦 Technology Stack

### Backend
- **Framework**: ASP.NET 6.0
- **ORM**: Dapper
- **Authentication**: JWT Bearer
- **Monitoring**: Prometheus

### Frontend
- **Framework**: React 18
- **Build Tool**: Vite
- **HTTP Client**: Axios
- **Router**: React Router v7
- **UI Icons**: Lucide React

### Infrastructure
- **Container**: Docker & Docker Compose
- **Database**: MySQL 8.0
- **Monitoring**: Prometheus

## 🧪 Testing

Run backend tests:
```bash
cd backend.tests
dotnet test
```

## 📊 Useful Commands

### Build Images Only
```bash
docker-compose build
```

### View Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f backend
```

### SSH into Container
```bash
docker exec -it mybrick-backend bash
docker exec -it mybrick-db bash
```

### Frontend Build
```bash
cd frontend
npm run build
npm run preview
```

### Backend Validation
```bash
cd backend
dotnet build
dotnet test (if tests exist)
```

## 🔄 Managing Data

### Backup Database
```bash
docker exec mybrick-db mysqldump -u mybrick_user -pMyBrick@2026 ConstructionSaaS > backup.sql
```

### Restore Database
```bash
docker exec -i mybrick-db mysql -u mybrick_user -pMyBrick@2026 ConstructionSaaS < backup.sql
```

## 🛠️ Development Workflow

1. **Start Docker containers**
   ```bash
   docker-compose up
   ```

2. **For live development**, run frontend and backend locally:
   ```bash
   # Terminal 1: Backend
   cd backend && dotnet run
   
   # Terminal 2: Frontend
   cd frontend && npm run dev
   ```

3. **Make code changes** - they will hot-reload
4. **Stop containers** when done:
   ```bash
   docker-compose down
   ```

## 📝 Environment Variables

Create a `.env` file in the root directory if needed:

```env
# Database
DB_USER=mybrick_user
DB_PASSWORD=MyBrick@2026
DB_DATABASE=ConstructionSaaS

# JWT
JWT_KEY=super_secret_jwt_key_for_mybrick_construction_platform_2026

# API
API_PORT=5154
FRONTEND_PORT=80
```

## 🐛 Troubleshooting

### Port Already in Use
```bash
# Check which process is using the port
lsof -i :PORT_NUMBER

# Kill the process
kill -9 <PID>
```

### Database Connection Issues
- Verify MySQL container is running: `docker ps`
- Check logs: `docker-compose logs db`
- Ensure credentials match in `appsettings.json`

### Frontend Not Loading
- Clear browser cache (Ctrl+Shift+Delete)
- Check frontend logs: `docker-compose logs frontend`
- Verify API is running: `curl http://localhost:5154/swagger`

### Package Issues
```bash
# Clear npm cache
npm cache clean --force

# Reinstall dependencies
rm -rf node_modules package-lock.json
npm install
```

## 📚 API Documentation

Swagger API documentation is available at:
```
http://localhost:5154/swagger
```

## 📞 Support

For issues or questions about the project setup, check:
1. Docker Compose logs
2. Application error logs
3. Database connection strings
4. Environment variables

---

**Last Updated**: May 2026
