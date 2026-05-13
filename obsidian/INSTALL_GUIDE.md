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
> Stack: .NET 10 + React/Vite + MySQL 8 + Docker + Jenkins + Terraform + Ansible + Kubernetes + Prometheus + Grafana

---

## Prerequisites

Install all tools before starting.

### Required Tools

| Tool | Version | Install |
|------|---------|---------|
| Git | ≥ 2.x | https://git-scm.com |
| Docker Desktop / OrbStack | latest | https://orbstack.dev |
| .NET SDK | 10.0 | https://dotnet.microsoft.com/download/dotnet/10.0 |
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

Deploys all services: backend (2 replicas), frontend (2 replicas), MySQL, Prometheus, Grafana.

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
mysql-xxxxxxxxx-xxxxx               1/1     Running   0          2m
prometheus-xxxxxxxxx-xxxxx          1/1     Running   0          2m
grafana-xxxxxxxxx-xxxxx             1/1     Running   0          2m
```

Verify services:

```bash
kubectl get svc -n production
```

```
mybrick-backend-svc    NodePort   ...   5154:30154/TCP
mybrick-frontend-svc   NodePort   ...   80:30080/TCP
mysql-svc              ClusterIP  ...   3306/TCP
prometheus-svc         NodePort   ...   9090:30090/TCP
grafana-svc            NodePort   ...   3000:30300/TCP
```

Access via Kubernetes:

| Service | URL |
|---------|-----|
| Frontend | http://localhost:30080 |
| Backend | http://localhost:30154 |
| Metrics | http://localhost:30154/metrics |
| Prometheus | http://localhost:30090 |
| Grafana | http://localhost:30300 |

---

## Step 7 — Verify Monitoring Stack

Prometheus and Grafana are auto-deployed in Kubernetes with ConfigMaps.

### Grafana — Auto-Provisioned

```bash
kubectl get pods -n production -l app=grafana
# grafana-xxxxxxxxx-xxxxx   1/1     Running
```

Login to Grafana:

```
URL: http://localhost:30300
Username: admin
Password: admin
```

### Dashboard & Datasource — Auto-Configured

1. **Datasources** — Prometheus already connected at `http://prometheus-svc:9090` ✅
2. **Dashboards → MyBrick — API Dashboard** — auto-imported ✅

Dashboard displays 6 live panels:
- Request Rate (req/s)
- Error Rate (failed/s)
- Latency p95
- Pod Health
- Total Projects
- Total Expenses (THB)

### Prometheus Verification

```bash
curl http://localhost:30090/api/v1/targets
# Should show: mybrick-backend health = "up"

curl http://localhost:30090/api/v1/query?query=up
# Should return metric value = 1 (healthy)
```

See [[SEALED_SECRETS]] for credential management (app secrets + MySQL passwords encrypted in K8s).

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

- [x] `app/backend/` — .NET 10 API with `/metrics`
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
- [x] `k8s/prometheus.yaml` + `grafana.yaml` + `grafana-dashboard-configmap.yaml`
- [x] `k8s/mysql.yaml` + `mysql-init-job.yaml`
- [x] `k8s/sealed-app-secrets.yaml` + `sealed-mysql-credentials.yaml`
- [x] `README.md`

### Pipeline Verified

- [ ] Push to `main` triggers Jenkins automatically
- [ ] All 5 Jenkins stages pass (green)
- [ ] Docker images pushed to Docker Hub (`mikedeni/mybrick-backend`, `mikedeni/mybrick-frontend`)
- [ ] Pods running in Kubernetes (`STATUS: Running`)
- [ ] Frontend accessible at `http://localhost:30080`
- [ ] Backend accessible at `http://localhost:30154`
- [ ] `/metrics` returns Prometheus data
- [ ] Prometheus running in K8s (`kubectl get pods -n production` shows prometheus pod)
- [ ] Prometheus target `mybrick-backend` shows **UP** (`curl http://localhost:30090/api/v1/targets`)
- [ ] Grafana running in K8s (`kubectl get pods -n production` shows grafana pod)
- [ ] Grafana dashboard auto-provisioned at http://localhost:30300
- [ ] All 6 dashboard panels show live metrics (Request Rate, Error Rate, Latency, Pod Health, Projects, Expenses)

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
curl http://localhost:30154/metrics
# Must return prometheus text format
# If empty — MapMetrics() not registered in Program.cs
```

**Grafana dashboard shows "No data"**

```bash
# Check Prometheus datasource URL
curl -u admin:admin http://localhost:30300/api/datasources
# url should be: http://prometheus-svc:9090

# Check Prometheus has metrics
curl http://localhost:30090/api/v1/query?query=microsoft_aspnetcore_hosting_total_requests
# Should return non-empty result array

# Reload Grafana dashboard
kubectl rollout restart deployment/grafana -n production
```

**MySQL connection string not working**

```bash
# Check sealed secrets auto-decrypted
kubectl get secrets -n production
# Should show: app-secrets, mysql-credentials (decrypted from sealed versions)

kubectl get secret app-secrets -n production -o jsonpath='{.data}' | base64 -d
# Verify ConnectionString key exists
```

---

## 🔗 Related

- [[HOME]] — Vault index
- [[PIPELINE_CHECKLIST]] — Tick-box version of this guide
- [[COMMANDS_CHEATSHEET]] — All commands in one place
- [[MANUAL_RUN]] — Alternative run options (dev mode, per-service)
- [[DEMO_GUIDE]] — After install: how to demo
- [[PROJECT_GUIDE]] — Phase completion checklist
