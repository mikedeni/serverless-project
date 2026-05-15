---
title: MyBrick — Installation Guide (Windows)
tags:
  - guide
  - install
  - setup
  - windows
  - deliverable
course: ENG23 3074
---

# 🔧 MyBrick — Installation Guide (Windows)

> Complete setup guide to run the full MyBrick pipeline on Windows and verify all deliverables.
> Stack: .NET 10 + React/Vite + MySQL 8 + Docker + Jenkins + Terraform + Ansible + Kubernetes + Prometheus + Grafana

---

## Prerequisites

Install all tools before starting. Use **PowerShell (Admin)** for all commands.

### Required Tools

| Tool | Version | Install |
|------|---------|---------|
| Git | ≥ 2.x | https://git-scm.com/download/win |
| Docker Desktop | latest | https://www.docker.com/products/docker-desktop |
| .NET SDK | 10.0 | https://dotnet.microsoft.com/download/dotnet/10.0 |
| Node.js | ≥ 20.x | https://nodejs.org (Windows x64 .msi) |
| kubectl | ≥ 1.28 | https://kubernetes.io/docs/tasks/tools/install-kubectl-windows |
| Terraform | ≥ 1.x | https://www.terraform.io/downloads.html (Windows x64) |
| Ansible | ≥ 2.15 | Windows Subsystem for Linux (WSL2) or `pip install ansible` |
| Jenkins | ≥ 2.4xx | Docker container (see Step 8.1) |

### Verify Installations

Open **PowerShell (Admin)** and run:

```powershell
git --version
docker --version
dotnet --version
node --version
kubectl version --client
terraform --version
# For Ansible via WSL2: wsl ansible --version
```

---

## Step 1 — Clone Repository

```powershell
git clone https://github.com/mikedeni/serverless-project.git
cd serverless-project
```

Verify structure:

```powershell
ls
# Jenkinsfile  README.md  ansible/  app/  docs/  k8s/  monitoring/  terraform/
```

---

## Step 2 — Run App (Docker Compose)

```powershell
cd app
docker compose up -d
```

Wait ~30–60 seconds for MySQL to initialize, then verify:

```powershell
docker compose ps
```

All four containers must show `running`:

```
NAME                 STATUS
mybrick-db           running
mybrick-backend      running
mybrick-frontend     running
mybrick-prometheus   running
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

```powershell
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

Requires Docker Desktop with Kubernetes enabled (Settings → Kubernetes → Enable Kubernetes).

Verify Kubernetes context:

```powershell
kubectl config get-contexts
# Should show: docker-desktop (current context)
```

Switch to Docker Desktop context:

```powershell
kubectl config use-context docker-desktop
kubectl get nodes
# Should show: docker-desktop    Ready    control-plane,master
```

Then provision:

```powershell
cd terraform
terraform init
terraform plan
terraform apply
```

Terraform creates namespace `production` in the cluster.

Verify:

```powershell
kubectl get namespace production
```

---

## Step 5 — Configure Environment (Ansible)

**Option A: Use WSL2 (Recommended)**

```bash
# From Windows PowerShell, open WSL2 terminal
wsl

# Inside WSL2:
cd /mnt/c/Users/<YourUsername>/Projects/serverless-project
cd ansible
ansible-playbook -i inventory playbook.yml
```

**Option B: Install Ansible via Python (Windows Native)**

```powershell
# Install Python 3.10+ first (https://python.org)
pip install ansible

cd ansible
ansible-playbook -i inventory playbook.yml
```

Ansible verifies kubectl, sets context to `docker-desktop`, and ensures namespace exists.

---

## Step 6 — Deploy to Kubernetes

```powershell
kubectl apply -f k8s/
```

Deploys all services: backend (2 replicas), frontend (2 replicas), MySQL, Prometheus, Grafana.

Monitor rollout:

```powershell
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

```powershell
kubectl get svc -n production
```

```
NAME                   TYPE       CLUSTER-IP      EXTERNAL-IP   PORT(S)
mybrick-backend-svc    NodePort   10.x.x.x        <pending>     5154:30154/TCP
mybrick-frontend-svc   NodePort   10.x.x.x        <pending>     80:30080/TCP
mysql-svc              ClusterIP  10.x.x.x        <none>        3306/TCP
prometheus-svc         NodePort   10.x.x.x        <pending>     9090:30090/TCP
grafana-svc            NodePort   10.x.x.x        <pending>     3000:30300/TCP
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

```powershell
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

```powershell
curl http://localhost:30090/api/v1/targets
# Should show: mybrick-backend health = "up"

curl http://localhost:30090/api/v1/query?query=up
# Should return metric value = 1 (healthy)
```

> **Note:** If `curl` unavailable, use `Invoke-WebRequest` in PowerShell:
> ```powershell
> Invoke-WebRequest -Uri "http://localhost:30090/api/v1/targets" | Select-Object -ExpandProperty Content
> ```

See [[SEALED_SECRETS]] for credential management (app secrets + MySQL passwords encrypted in K8s).

---

## Step 8 — Setup Jenkins CI/CD

### 8.1 Run Jenkins

```powershell
docker run -d `
  --name jenkins `
  -p 8080:8080 -p 50000:50000 `
  -v jenkins_home:/var/jenkins_home `
  -v /var/run/docker.sock:/var/run/docker.sock `
  jenkins/jenkins:lts
```

Get initial admin password:

```powershell
docker exec jenkins cat /var/jenkins_home/secrets/initialAdminPassword
```

Open http://localhost:8080 and complete setup wizard.

### 8.1b Configure Jenkins URL

After setup wizard, go to **Manage Jenkins → System** (or `http://localhost:8080/configure`):

Set **Instance Configuration → Jenkins URL:**
```
http://localhost:8080/
```

This URL is required for:
- Email notifications
- GitHub PR status updates
- BUILD_URL environment variable

For webhooks from GitHub: use `http://localhost:8080/github-webhook/`

### 8.2 Install Plugins

Manage Jenkins → Plugins → Available → install:

- Git Plugin
- Pipeline
- GitHub Integration Plugin

### 8.2.5 Install Build Tools in Jenkins Container

Jenkins container needs: dotnet, docker, terraform, kubectl, ansible

**Note:** Jenkins Docker image runs Linux; install tools inside container via `docker exec`.

```powershell
# Install dotnet 10.0
docker exec -u 0 jenkins bash -c '
  curl -L https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
  chmod +x dotnet-install.sh
  ./dotnet-install.sh --channel 10.0 --install-dir /usr/local/dotnet
  ln -s /usr/local/dotnet/dotnet /usr/local/bin/dotnet
  apt-get update && apt-get install -y libicu*
'

# Install kubectl, terraform, ansible
docker exec -u 0 jenkins bash -c '
  curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/x86_64/kubectl"
  chmod +x kubectl && mv kubectl /usr/local/bin/
  apt-get install -y ansible
'

# Install terraform
docker exec -u 0 jenkins bash -c '
  TERRAFORM_VERSION=$(curl -s https://api.github.com/repos/hashicorp/terraform/releases/latest | grep -o "\"tag_name\": \"v[0-9.]*\"" | head -1 | cut -d"\"" -f4 | sed "s/v//")
  curl -fsSL https://releases.hashicorp.com/terraform/${TERRAFORM_VERSION}/terraform_${TERRAFORM_VERSION}_linux_x86_64.zip -o terraform.zip
  unzip -o terraform.zip -d /usr/local/bin/ && rm terraform.zip
'

# Fix docker socket permissions
docker exec -u 0 jenkins chmod 666 /var/run/docker.sock
```

### 8.2.6 Setup kubeconfig in Jenkins

Copy kubeconfig from host → Jenkins container:

```powershell
# Export kubeconfig
kubectl config view --raw > $env:TEMP\kubeconfig.yaml

# Copy to Jenkins
docker exec -u 0 jenkins mkdir -p /var/jenkins_home/.kube
docker cp $env:TEMP\kubeconfig.yaml jenkins:/var/jenkins_home/.kube/config
docker exec -u 0 jenkins bash -c 'chown jenkins:jenkins /var/jenkins_home/.kube /var/jenkins_home/.kube/config && chmod 600 /var/jenkins_home/.kube/config'
```

Update kubeconfig server IP (Docker Desktop uses `kubernetes.docker.internal`):

```powershell
docker exec jenkins sed -i 's|https://127.0.0.1:[0-9]*|https://kubernetes.docker.internal:6443|g' /var/jenkins_home/.kube/config
```

### 8.3 Add Credentials

Manage Jenkins → Credentials → Global → Add Credential:

| ID | Type | Value |
|----|------|-------|
| `dockerhub-credentials` | Username/Password | Docker Hub: mikedeni + password |
| `github-token` | Secret text | GitHub personal access token |

### 8.4 Create Pipeline Job

1. New Item → **Pipeline** → name: `mybrick`
2. Pipeline → Definition: **Pipeline script from SCM**
3. SCM: Git → Repository URL: `https://github.com/mikedeni/serverless-project.git`
4. Branch: `*/main`
5. Script Path: `Jenkinsfile`
6. Save

### 8.5 Setup GitHub Webhook

1. GitHub repo → **Settings → Webhooks → Add webhook**
2. Payload URL: `http://[your-jenkins-host]:8080/github-webhook/`
3. Content type: `application/json`
4. Trigger: **Just the push event**
5. Jenkins job → **Build Triggers** → tick **GitHub hook trigger for GITScm polling**

### 8.6 Verify Pipeline

Push any change to `main` → Jenkins auto-triggers. All 5 stages must pass:

```
Checkout ✅ → Test ✅ → Docker Build ✅ → Push to Hub ✅ → Deploy ✅
```

### 8.7 Terraform Configuration Notes

- `terraform/main.tf` uses data source (not resource) to reference existing `production` namespace created by ansible
- Deploy stage order: **Ansible → Terraform → kubectl** (ansible creates namespace first)
- Provider uses `insecure = true` for local K8s clusters (skips TLS verification for self-signed certs)

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

```powershell
docker compose logs backend
# Usually: DB not ready yet
docker compose restart backend
```

**dotnet test — build error**

```powershell
cd app/backend
dotnet restore
cd ..\backend.tests
dotnet restore
dotnet test
```

**Terraform init fails (no provider)**

```powershell
terraform init -upgrade
```

**Kubernetes — ImagePullBackOff**

```powershell
kubectl describe pod <pod-name> -n production
# Ensure mikedeni/mybrick-* repos are Public on Docker Hub
```

**Jenkins cannot access Docker (Windows)**

Ensure Docker Desktop is running and WSL2 backend enabled:
- Settings → Resources → WSL integration
- Restart Docker Desktop

**Prometheus target DOWN**

```powershell
curl http://localhost:30154/metrics
# OR from PowerShell:
Invoke-WebRequest -Uri "http://localhost:30154/metrics"

# Must return prometheus text format
# If empty — MapMetrics() not registered in Program.cs
```

**Grafana dashboard shows "No data"**

```powershell
# Check Prometheus datasource URL
Invoke-WebRequest -Uri "http://localhost:30300/api/datasources" -Authentication Basic -Credential (New-Object PSCredential 'admin', (ConvertTo-SecureString 'admin' -AsPlainText -Force)) | ConvertFrom-Json
# url should be: http://prometheus-svc:9090

# Check Prometheus has metrics
curl http://localhost:30090/api/v1/query?query=microsoft_aspnetcore_hosting_total_requests

# Reload Grafana dashboard
kubectl rollout restart deployment/grafana -n production
```

**MySQL connection string not working**

```powershell
# Check sealed secrets auto-decrypted
kubectl get secrets -n production
# Should show: app-secrets, mysql-credentials (decrypted from sealed versions)

kubectl get secret app-secrets -n production -o jsonpath='{.data}' | base64 -d
# Verify ConnectionString key exists
```

**Docker Desktop Kubernetes not starting**

1. Settings → Resources → Memory: increase to ≥ 6 GB
2. Settings → Resources → CPUs: increase to ≥ 4
3. Settings → Kubernetes → Reset Kubernetes cluster
4. Restart Docker Desktop

---

## Windows-Specific Notes

### PowerShell vs Command Prompt

All commands in this guide use **PowerShell**. If using Command Prompt (`cmd.exe`), translate backticks (`` ` ``) to `^`:

```cmd
REM PowerShell: docker run -d `
REM CMD: docker run -d ^
```

### WSL2 for Ansible

Windows native Ansible support is limited. Use WSL2:

1. Enable WSL2: `wsl --install -d Ubuntu-22.04` (requires Windows 11 or latest Windows 10)
2. Enable Docker Desktop WSL2 backend (default)
3. Access project from WSL: `cd /mnt/c/Users/<YourUsername>/Projects/serverless-project`

### Paths in PowerShell

Use forward slashes `/` or backslashes `\`:

```powershell
# Both work:
cd terraform
cd .\terraform
cd terraform/
```

### Long Paths

Windows has 260-character path limits. If cloning to deeply nested folder, enable long paths:

```powershell
New-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Control\FileSystem" -Name "LongPathsEnabled" -Value 1 -PropertyType DWORD -Force
```

---

## 🔗 Related

- [[INSTALL_GUIDE]] — macOS/Linux version
- [[HOME]] — Vault index
- [[PIPELINE_CHECKLIST]] — Tick-box version
- [[COMMANDS_CHEATSHEET]] — All commands in one place
- [[DEMO_GUIDE]] — After install: how to demo
- [[PROJECT_GUIDE]] — Phase completion checklist
