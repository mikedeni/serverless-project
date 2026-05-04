---
title: ENG23 3074 — Project Completion Guide
tags:
  - guide
  - devops
  - todo
course: ENG23 3074
status: in-progress
---

# ENG23 3074 — Project Completion Guide

> [!abstract] Stack
> Git → Jenkins → Docker → Terraform/Ansible → Kubernetes (OrbStack) → Prometheus/Grafana

**App:** MyBrick — Construction SaaS platform
**Stack:** .NET 8 (ASP.NET Core) + React/Vite + MySQL 8 + Docker Compose

---

## Checklist Overview

- [ ] [[#Phase 1 — App + Git]]
- [ ] [[#Phase 2 — Jenkins CI/CD]]
- [ ] [[#Phase 3 — Terraform + Ansible]]
- [ ] [[#Phase 4 — Kubernetes]]
- [ ] [[#Phase 5 — Monitoring]]
- [ ] [[#README Completion]]
- [ ] [[#Final Deliverables]]

---

## Phase 1 — App + Git

### 1.1 App Overview

```
app/
├── backend/                  # ASP.NET Core Web API (.NET 8)
│   ├── Controllers/          # REST endpoints (Auth, Projects, Workers, ...)
│   ├── Services/             # Business logic
│   ├── Repositories/         # Dapper data access (MySQL)
│   ├── Models/               # Domain models
│   ├── DTOs/                 # Request/response shapes
│   ├── Data/                 # DapperContext (conn factory)
│   ├── Monitoring/           # BusinessMetrics (prometheus-net)
│   ├── Program.cs            # App bootstrap + DI
│   ├── appsettings.json
│   └── Dockerfile
├── backend.tests/            # xUnit test project
│   ├── ProjectServiceTests.cs
│   ├── DashboardServiceTests.cs
│   └── ReportServiceTests.cs
├── frontend/                 # React + Vite
│   ├── src/
│   │   ├── pages/            # Full-page views
│   │   ├── components/       # Shared UI components
│   │   ├── contexts/         # AuthContext (JWT)
│   │   └── utils/api.js      # Axios wrapper
│   └── Dockerfile
├── database/
│   ├── schema.sql            # Full DDL (auto-runs in Docker)
│   └── update.sql            # Migration patches
├── docker-compose.yml        # db + backend + frontend + prometheus
└── prometheus.yml            # Scrape config for mybrick-backend
```

### 1.2 API Endpoints

| Controller | Routes |
|------------|--------|
| Auth | `POST /api/auth/login`, `POST /api/auth/register` |
| Projects | CRUD `/api/projects` |
| Workers | CRUD `/api/workers` |
| Materials | CRUD `/api/materials` |
| Tasks | CRUD `/api/tasks` |
| Dashboard | `GET /api/dashboard` |
| Reports | `GET /api/reports/*` |
| Expenses | CRUD `/api/expenses` |
| Invoices | CRUD `/api/invoices` |
| Quotations | CRUD `/api/quotations` |
| Notifications | `/api/notifications` |
| Documents | `/api/documents` |
| DailyReports | `/api/dailyreports` |
| Attendances | `/api/attendances` |
| Subcontractors | `/api/subcontractors` |
| Metrics | `GET /metrics` (Prometheus) |

### 1.3 Git Setup

```bash
git remote add origin https://github.com/[username]/serverless-project.git

# Branch strategy
git checkout -b dev
git checkout -b feature/[feature-name]
```

Branch rules:
- `main` → protected, triggers pipeline on merge
- `dev` → integration, merge `feature/*` here first
- `feature/*` → daily work

---

## Phase 2 — Jenkins CI/CD

### 2.1 Install Jenkins

```bash
docker run -d \
  --name jenkins \
  -p 8080:8080 -p 50000:50000 \
  -v jenkins_home:/var/jenkins_home \
  -v /var/run/docker.sock:/var/run/docker.sock \
  jenkins/jenkins:lts

docker exec jenkins cat /var/jenkins_home/secrets/initialAdminPassword
```

### 2.2 Required Jenkins Plugins

Install via **Manage Jenkins → Plugins**:

- [x] Git Plugin
- [x] Pipeline
- [x] Docker Pipeline
- [x] GitHub Integration Plugin

### 2.3 Add Credentials

**Manage Jenkins → Credentials → Global → Add:**

| ID | Type | Value |
|----|------|-------|
| `dockerhub-credentials` | Username/Password | Docker Hub login |
| `github-token` | Secret text | GitHub personal access token |

### 2.4 Jenkinsfile Template

```groovy
pipeline {
    agent any

    environment {
        BACKEND_IMAGE  = "mikedeni/mybrick-backend"
        FRONTEND_IMAGE = "mikedeni/mybrick-frontend"
        IMAGE_TAG      = "${BUILD_NUMBER}"
    }

    stages {
        stage('Checkout') {
            steps { checkout scm }
        }

        stage('Test') {
            steps {
                sh 'dotnet test app/backend.tests/ConstructionSaaS.Tests.csproj --no-build -v n'
            }
        }

        stage('Docker Build') {
            steps {
                sh "docker build -t ${BACKEND_IMAGE}:${IMAGE_TAG} ./app/backend"
                sh "docker build -t ${FRONTEND_IMAGE}:${IMAGE_TAG} ./app/frontend"
            }
        }

        stage('Push to Hub') {
            steps {
                withCredentials([usernamePassword(
                    credentialsId: 'dockerhub-credentials',
                    usernameVariable: 'DOCKER_USER',
                    passwordVariable: 'DOCKER_PASS'
                )]) {
                    sh """
                        echo $DOCKER_PASS | docker login -u $DOCKER_USER --password-stdin
                        docker push ${BACKEND_IMAGE}:${IMAGE_TAG}
                        docker push ${FRONTEND_IMAGE}:${IMAGE_TAG}
                    """
                }
            }
        }

        stage('Deploy') {
            steps {
                sh 'cd terraform && terraform init && terraform apply -auto-approve'
                sh 'cd ansible && ansible-playbook -i inventory playbook.yml'
                sh "kubectl apply -f k8s/"
            }
        }
    }

    post {
        always { sh 'docker logout' }
    }
}
```

### 2.5 GitHub Webhook Setup

1. GitHub repo → **Settings → Webhooks → Add webhook**
2. Payload URL: `http://[jenkins-host]:8080/github-webhook/`
3. Content type: `application/json`
4. Trigger: **Just the push event**
5. Jenkins job → **Build Triggers** → tick **GitHub hook trigger for GITScm polling**

---

## Phase 3 — Terraform + Ansible

### 3.1 Terraform — Provision Namespace

```hcl
# terraform/main.tf
terraform {
  required_providers {
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.0"
    }
  }
}

provider "kubernetes" {
  config_path = "~/.kube/config"
}

resource "kubernetes_namespace" "app_ns" {
  metadata {
    name = var.namespace
  }
}
```

```hcl
# terraform/variables.tf
variable "namespace" { default = "production" }
variable "app_name"  { default = "mybrick" }
```

```hcl
# terraform/outputs.tf
output "namespace" {
  value = kubernetes_namespace.app_ns.metadata[0].name
}
```

```bash
cd terraform && terraform init && terraform plan && terraform apply
```

### 3.2 Ansible — Configure Environment

```yaml
# ansible/inventory
[local]
localhost ansible_connection=local
```

```yaml
# ansible/playbook.yml
---
- name: Configure deployment environment
  hosts: local
  tasks:
    - name: Verify kubectl available
      command: kubectl version --client

    - name: Set kubeconfig context
      command: kubectl config use-context orbstack

    - name: Verify cluster reachable
      command: kubectl cluster-info
```

---

## Phase 4 — Kubernetes

### 4.1 Verify OrbStack Cluster

```bash
kubectl config use-context orbstack
kubectl get nodes
# Expected: orbstack   Ready   control-plane,master
```

### 4.2 Backend Deployment

```yaml
# k8s/backend-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: mybrick-backend
  namespace: production
spec:
  replicas: 2
  selector:
    matchLabels:
      app: mybrick-backend
  template:
    metadata:
      labels:
        app: mybrick-backend
    spec:
      containers:
        - name: mybrick-backend
          image: mikedeni/mybrick-backend:latest
          ports:
            - containerPort: 5154
          env:
            - name: ConnectionStrings__DefaultConnection
              value: "Server=mybrick-db;Database=ConstructionSaaS;User=mybrick_user;Password=MyBrick@2026;"
            - name: Jwt__Key
              value: "super_secret_jwt_key_for_mybrick_construction_platform_2026"
          readinessProbe:
            httpGet:
              path: /metrics
              port: 5154
            initialDelaySeconds: 10
            periodSeconds: 10
```

### 4.3 Frontend Deployment

```yaml
# k8s/frontend-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: mybrick-frontend
  namespace: production
spec:
  replicas: 2
  selector:
    matchLabels:
      app: mybrick-frontend
  template:
    metadata:
      labels:
        app: mybrick-frontend
    spec:
      containers:
        - name: mybrick-frontend
          image: mikedeni/mybrick-frontend:latest
          ports:
            - containerPort: 80
```

### 4.4 Services

```yaml
# k8s/services.yaml
apiVersion: v1
kind: Service
metadata:
  name: mybrick-backend-svc
  namespace: production
spec:
  type: NodePort
  selector:
    app: mybrick-backend
  ports:
    - port: 5154
      targetPort: 5154
      nodePort: 30154
---
apiVersion: v1
kind: Service
metadata:
  name: mybrick-frontend-svc
  namespace: production
spec:
  type: NodePort
  selector:
    app: mybrick-frontend
  ports:
    - port: 80
      targetPort: 80
      nodePort: 30080
```

### 4.5 Verify Deploy

```bash
kubectl apply -f k8s/
kubectl get pods -n production
kubectl get svc  -n production

# Access
# Frontend: http://localhost:30080
# Backend:  http://localhost:30154
# Metrics:  http://localhost:30154/metrics
```

---

## Phase 5 — Monitoring

### 5.1 Prometheus — Already Wired

Backend uses `prometheus-net.AspNetCore`. Metrics auto-exposed at `/metrics`.

`app/prometheus.yml` scrapes `backend:5154/metrics` every 15s — already configured.

Custom business metrics in `app/backend/Monitoring/BusinessMetrics.cs`.

### 5.2 Run Prometheus (Docker Compose)

```bash
cd app && docker compose up prometheus
# UI: http://localhost:9090
```

### 5.3 Grafana Setup

```bash
docker run -d -p 3000:3000 grafana/grafana
# UI: http://localhost:3000 (admin/admin)
```

1. Add datasource: **Prometheus** → `http://localhost:9090`
2. Create dashboard:

| Panel | PromQL |
|-------|--------|
| Request Rate | `rate(http_requests_received_total[1m])` |
| Error Rate | `rate(http_requests_received_total{code=~"5.."}[1m])` |
| Latency p95 | `histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))` |
| Pod Health | `up{job="mybrick-backend"}` |

3. Export dashboard JSON → `monitoring/grafana-dashboard.json`

---

## README Completion

```bash
grep -n "\[" README.md
```

| Placeholder | Replace with |
|-------------|-------------|
| `[username]` | GitHub username |
| `[project-name]` | `serverless-project` |
| `[app-name]` | `mybrick` |
| `[namespace]` | `production` |
| `[ชื่อ นามสกุล]` | real team member names + IDs |

---

## Final Deliverables

> [!todo] Before submission — verify all checked

### Files in Repo

- [x] `app/backend/` — .NET 8 API with `/metrics`
- [x] `app/backend/Dockerfile`
- [x] `app/backend.tests/` — xUnit tests
- [x] `app/frontend/` — React/Vite
- [x] `app/frontend/Dockerfile`
- [x] `app/database/schema.sql`
- [x] `app/docker-compose.yml`
- [x] `app/prometheus.yml`
- [ ] `Jenkinsfile`
- [ ] `terraform/main.tf` + `variables.tf` + `outputs.tf`
- [ ] `ansible/inventory` + `playbook.yml`
- [ ] `k8s/backend-deployment.yaml` + `frontend-deployment.yaml` + `services.yaml`
- [ ] `monitoring/grafana-dashboard.json`
- [ ] `README.md` — zero `[placeholder]` remaining

### Pipeline Works

- [ ] Push `feature/*` → merge `dev` → merge `main` → triggers Jenkins
- [ ] All Jenkins stages pass (green)
- [ ] Docker images pushed to Docker Hub
- [ ] Pods running in Kubernetes (`STATUS: Running`)
- [ ] Frontend accessible at `http://localhost:30080`
- [ ] Backend accessible at `http://localhost:30154`
- [ ] `/metrics` returns Prometheus data
- [ ] Grafana dashboard shows live data

### Demo Prep

- [ ] Architecture diagram matches actual setup
- [ ] Can explain each stage in pipeline
- [ ] Live demo: push code → watch pipeline → app updates

---

## Troubleshooting Quick Ref

**Pods stuck `Pending`**
```bash
kubectl describe pod [pod-name] -n production
# Check Events section
```

**Backend won't connect to MySQL**
```bash
# Verify DB pod running
kubectl get pods -n production | grep db
# Check connection string env var matches DB service name
```

**Jenkins Docker Build fails**
```bash
sudo usermod -aG docker jenkins
sudo systemctl restart jenkins
```

**Prometheus target `DOWN`**
```bash
curl http://localhost:30154/metrics
# If empty → prometheus-net middleware not registered in Program.cs
```

**Terraform apply fails (namespace exists)**
```bash
terraform import kubernetes_namespace.app_ns production
```

**dotnet test fails (DB dependency)**
```bash
# Tests use mocked services — no DB needed
# Check GlobalUsings.cs has xUnit + Moq references
```

---

## Team Task Split (Suggested)

| Member | Owns |
|--------|------|
| Member 1 | App code (Phase 1) + README |
| Member 2 | Jenkins + Dockerfiles (Phase 2) |
| Member 3 | Terraform + Ansible (Phase 3) |
| Member 4 | Kubernetes + Monitoring (Phase 4–5) |

> [!warning] Everyone must understand the full pipeline for the demo — not just their own part.
