---
title: MyBrick — Installation Guide
tags:
  - guide
  - install
  - setup
  - deliverable
course: ENG23 3074
---

# 🔧 MyBrick — Installation Guide

> Complete setup guide to run the full MyBrick pipeline and verify all deliverables.
> Stack: .NET 8 + React/Vite + MySQL 8 + Docker + Jenkins + Terraform + Ansible + Kubernetes + Prometheus + Grafana

---

## Prerequisites

Install all tools before starting.

### Required Tools

| Tool | Version | Install |
|------|---------|---------|
| Git | ≥ 2.x | https://git-scm.com |
| Docker Desktop / OrbStack | latest | https://orbstack.dev |
| .NET SDK | 8.0 | https://dotnet.microsoft.com/download/dotnet/8.0 |
| Node.js | ≥ 20.x | https://nodejs.org |
| kubectl | ≥ 1.28 | https://kubernetes.io/docs/tasks/tools |
| Terraform | ≥ 1.x | https://developer.hashicorp.com/terraform/install |
| Ansible | ≥ 2.15 | `pip install ansible` |
| Jenkins | ≥ 2.4xx | https://www.jenkins.io/download |

### Verify installations

```bash
git --version
docker --version
dotnet --version
node --version
kubectl version --client
terraform --version
ansible --version
```

---

## Step 1 — Clone Repository

```bash
git clone https://github.com/mikedeni/serverless-project.git
cd serverless-project
```

Verify structure:

```bash
ls
# Jenkinsfile  README.md  ansible/  app/  docs/  k8s/  monitoring/  terraform/
```

---

## Step 2 — Run App (Docker Compose)

```bash
cd app
docker compose up -d
```

Wait ~30–60 seconds for MySQL to initialize, then verify:

```bash
docker compose ps
```

All four containers must show `running`:

```
mybrick-db          running
mybrick-backend     running
mybrick-frontend    running
mybrick-prometheus  running
```

### Access the app

| Service | URL | Status |
|---------|-----|--------|
| Frontend | http://localhost:80 | Login page |
| Backend Swagger | http://localhost:5154/swagger | API docs |
| Prometheus metrics | http://localhost:5154/metrics | Raw metrics |
| Prometheus UI | http://localhost:9090 | Target list |

> Swagger is available in development mode only. In Kubernetes production it is disabled.

---

## Step 3 — Run Tests

```bash
cd app
dotnet test backend.tests/ConstructionSaaS.Tests.csproj -v n
```

Expected output:

```
Test Run Successful.
Total tests: X
     Passed: X
```

---

## Step 4 — Provision Infrastructure (Terraform)

Requires OrbStack or Minikube running with a local Kubernetes cluster.

```bash
kubectl config use-context orbstack
kubectl get nodes
# orbstack   Ready   control-plane,master
```

Then provision:

```bash
cd terraform
terraform init
terraform plan
terraform apply
```

Terraform creates namespace `production` in the cluster.

Verify:

```bash
kubectl get namespace production
```

---

## Step 5 — Configure Environment (Ansible)

```bash
cd ansible
ansible-playbook -i inventory playbook.yml
```

Ansible verifies kubectl, sets context to `orbstack`, and ensures namespace exists.

---

## Step 6 — Deploy to Kubernetes

```bash
kubectl apply -f k8s/
```

Monitor rollout:

```bash
kubectl get pods -n production --watch
```

All pods must reach `Running`:

```
NAME                                READY   STATUS    RESTARTS   AGE
mybrick-backend-xxxxxxxxx-xxxxx     1/1     Running   0          2m
mybrick-backend-xxxxxxxxx-yyyyy     1/1     Running   0          2m
mybrick-frontend-xxxxxxxxx-aaaaa    1/1     Running   0          2m
mybrick-frontend-xxxxxxxxx-bbbbb    1/1     Running   0          2m
```

Verify services:

```bash
kubectl get svc -n production
```

```
mybrick-backend-svc    NodePort   ...   5154:30154/TCP
mybrick-frontend-svc   NodePort   ...   80:30080/TCP
```

Access via Kubernetes:

| Service | URL |
|---------|-----|
| Frontend | http://localhost:30080 |
| Backend | http://localhost:30154 |
| Metrics | http://localhost:30154/metrics |

---

## Step 7 — Setup Monitoring (Grafana)

Prometheus is already running in Docker Compose (port 9090).

### Run Grafana

```bash
docker run -d -p 3000:3000 --name grafana grafana/grafana
```

### Configure datasource

1. Open http://localhost:3000 (admin / admin)
2. **Connections → Data sources → Add → Prometheus**
3. URL: `http://host.docker.internal:9090`
4. Click **Save & Test** — must show "Data source is working"

### Import dashboard

1. **Dashboards → Import**
2. Upload `monitoring/grafana-dashboard.json`
3. Select Prometheus datasource → **Import**

Dashboard shows 6 panels: Request Rate, Error Rate, Latency p95, Pod Health, Total Projects, Total Expenses (THB).

---

## Step 8 — Setup Jenkins CI/CD

### 8.1 Run Jenkins

```bash
docker run -d \
  --name jenkins \
  -p 8080:8080 -p 50000:50000 \
  -v jenkins_home:/var/jenkins_home \
  -v /var/run/docker.sock:/var/run/docker.sock \
  jenkins/jenkins:lts
```

Get initial admin password:

```bash
docker exec jenkins cat /var/jenkins_home/secrets/initialAdminPassword
```

Open http://localhost:8080 and complete setup wizard.

### 8.2 Install plugins

Manage Jenkins → Plugins → Available → install:

- Git Plugin
- Pipeline
- Docker Pipeline
- GitHub Integration Plugin

### 8.3 Add credentials

Manage Jenkins → Credentials → Global → Add Credential:

| ID | Type | Value |
|----|------|-------|
| `dockerhub-credentials` | Username/Password | Docker Hub: mikedeni + password |
| `github-token` | Secret text | GitHub personal access token |

### 8.4 Create pipeline job

1. New Item → **Pipeline** → name: `mybrick`
2. Pipeline → Definition: **Pipeline script from SCM**
3. SCM: Git → Repository URL: `https://github.com/mikedeni/serverless-project.git`
4. Branch: `*/main`
5. Script Path: `Jenkinsfile`
6. Save

### 8.5 Setup GitHub webhook

1. GitHub repo → **Settings → Webhooks → Add webhook**
2. Payload URL: `http://[your-jenkins-host]:8080/github-webhook/`
3. Content type: `application/json`
4. Trigger: **Just the push event**
5. Jenkins job → **Build Triggers** → tick **GitHub hook trigger for GITScm polling**

### 8.6 Verify pipeline

Push any change to `main` → Jenkins auto-triggers. All 5 stages must pass:

```
Checkout ✅ → Test ✅ → Docker Build ✅ → Push to Hub ✅ → Deploy ✅
```

---

## Deliverables Checklist

Verify everything before submission:

### Files in Repository

- [x] `app/backend/` — .NET 8 API with `/metrics`
- [x] `app/backend/Dockerfile`
- [x] `app/backend.tests/` — xUnit tests
- [x] `app/frontend/` — React/Vite
- [x] `app/frontend/Dockerfile`
- [x] `app/database/schema.sql`
- [x] `app/docker-compose.yml`
- [x] `app/prometheus.yml`
- [x] `Jenkinsfile`
- [x] `terraform/main.tf` + `variables.tf` + `outputs.tf`
- [x] `ansible/inventory` + `playbook.yml`
- [x] `k8s/backend-deployment.yaml` + `frontend-deployment.yaml` + `services.yaml`
- [x] `monitoring/grafana-dashboard.json`
- [x] `README.md`

### Pipeline Verified

- [ ] Push to `main` triggers Jenkins automatically
- [ ] All 5 Jenkins stages pass (green)
- [ ] Docker images pushed to Docker Hub (`mikedeni/mybrick-backend`, `mikedeni/mybrick-frontend`)
- [ ] Pods running in Kubernetes (`STATUS: Running`)
- [ ] Frontend accessible at `http://localhost:30080`
- [ ] Backend accessible at `http://localhost:30154`
- [ ] `/metrics` returns Prometheus data
- [ ] Prometheus target `mybrick-backend` shows **UP**
- [ ] Grafana dashboard shows live data

### Demo Prep

- [ ] Can explain each Jenkins stage
- [ ] Can show live push → pipeline → deploy flow
- [ ] Architecture diagram matches actual setup (see README)

---

## Troubleshooting

**Docker Compose — backend exits immediately**

```bash
docker compose logs backend
# Usually: DB not ready yet
docker compose restart backend
```

**dotnet test — build error**

```bash
cd app/backend
dotnet restore
cd ../backend.tests
dotnet restore
dotnet test
```

**Terraform init fails (no provider)**

```bash
terraform init -upgrade
```

**Kubernetes — ImagePullBackOff**

```bash
kubectl describe pod <pod-name> -n production
# Ensure mikedeni/mybrick-* repos are Public on Docker Hub
```

**Jenkins cannot access Docker**

```bash
sudo usermod -aG docker jenkins
sudo systemctl restart jenkins
```

**Prometheus target DOWN**

```bash
curl http://localhost:5154/metrics
# Must return prometheus text format
# If empty — MapMetrics() not registered in Program.cs
```
