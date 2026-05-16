# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project: MyBrick

Construction management SaaS (ENG23 3074). .NET 8 Web API + React/Vite, containerized with Docker, deployed on Kubernetes via Jenkins CI/CD.

## Common Commands

### Backend (.NET 8)
```bash
cd app/backend
dotnet run                         # dev server → http://localhost:5154
dotnet build
```

### Frontend (React + Vite)
```bash
cd app/frontend
npm install
npm run dev                        # → http://localhost:5173
npm run build
npm run lint
```

### Tests (xUnit)
```bash
cd app
dotnet test backend.tests/ConstructionSaaS.Tests.csproj -v n   # verbose
dotnet test backend.tests/ConstructionSaaS.Tests.csproj -v minimal  # CI style
```

### Docker Compose (full stack)
```bash
cd app
docker compose up -d
# Frontend: http://localhost:80  Backend: http://localhost:5154
# Swagger:  http://localhost:5154/swagger  Metrics: http://localhost:5154/metrics
# Prometheus: http://localhost:9090
```

### Kubernetes
```bash
kubectl apply -f k8s/
kubectl get pods -n production
kubectl get svc  -n production
# Frontend: http://localhost:30080  Backend: http://localhost:30154
```

### Terraform & Ansible
```bash
cd terraform && terraform init && terraform plan && terraform apply
cd ansible   && ansible-playbook -i inventory playbook.yml
```

## Architecture

### Backend Pattern
`Controllers → Services → Repositories → Dapper → MySQL`

Each domain (projects, workers, materials, tasks, expenses, invoices, quotations, daily reports) has its own Controller/Service/Repository triple. Interfaces are defined per layer; DI is wired in `Program.cs`.

- `app/backend/Data/DapperContext.cs` — MySQL connection factory (singleton)
- `app/backend/Monitoring/BusinessMetrics.cs` — prometheus-net custom metrics
- `app/backend/Services/AuthService.cs` — JWT auth
- `app/backend/Services/PdfService.cs` — PDF generation (singleton)

### Frontend Pattern
React 18 + Axios. Auth state managed via `src/contexts/AuthContext.jsx` (JWT stored in memory). All API calls go through `src/utils/api.js` Axios wrapper.

### CI/CD Pipeline (Jenkinsfile)
`Checkout → [Test ‖ Docker Build backend ‖ Docker Build frontend] → Push to Hub → kubectl deploy`

Images: `mikedeni/mybrick-backend`, `mikedeni/mybrick-frontend`. Jenkins credential ID: `dockerhub-credentials`. Tag = `$BUILD_NUMBER`.

### Kubernetes (namespace: `production`)
- 2 replicas each for backend and frontend deployments
- Backend NodePort 30154, Frontend NodePort 30080
- Secrets via SealedSecrets (`k8s/sealed-*.yaml`)
- Prometheus + Grafana deployed in same namespace (`k8s/prometheus.yaml`, `k8s/grafana.yaml`)

### Monitoring
Prometheus scrapes `/metrics` from backend every 15s. Custom metrics: `mybrick_projects_total`, `mybrick_expenses_total_baht`. Grafana dashboard at `monitoring/grafana-dashboard.json`.

## Key Config

| Setting | Value |
|---------|-------|
| DB name | `ConstructionSaaS` |
| DB user | `mybrick_user` |
| Backend port | `5154` |
| K8s namespace | `production` |
| Prometheus scrape | `http://mybrick-backend-svc:5154/metrics` |

## Database

Schema: `app/database/schema.sql` (full DDL). Migrations: `app/database/update.sql`. MySQL 8 auto-initialized via Docker Compose volume mount.

## Branching

`main` (protected, triggers Jenkins) → `dev` (integration) → `feature/*`
