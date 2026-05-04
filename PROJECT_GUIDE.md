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

### 1.1 Choose Your App

Pick one simple app — keep it small, the pipeline is the point.

| Option | Stack | Endpoints needed |
|--------|-------|-----------------|
| Note API | Python Flask + SQLite | GET/POST/DELETE `/notes` |
| Task Tracker | Node.js Express + JSON | GET/POST/DELETE `/tasks` |
| URL Shortener | Python Flask | GET `/:code`, POST `/shorten` |

> [!tip] Recommendation
> **Note API (Python Flask)** — most examples online, easiest to add `/metrics`.

### 1.2 App Must Have

- [ ] `GET /` → health check, returns `{"status": "ok"}`
- [ ] `GET /metrics` → Prometheus metrics (use `prometheus_client` lib)
- [ ] At least 2 business endpoints (list + create)
- [ ] `requirements.txt` with pinned versions
- [ ] Dockerfile

### 1.3 Dockerfile Template

```dockerfile
FROM python:3.11-slim
WORKDIR /app
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt
COPY . .
EXPOSE 5000
CMD ["python", "app.py"]
```

### 1.4 Git Setup

```bash
git init
git remote add origin https://github.com/[username]/[project-name].git

# Create branches
git checkout -b dev
git checkout -b feature/initial-app
```

Branch rules:
- `main` → protected, trigger pipeline on merge
- `dev` → integration, merge feature/* here first
- `feature/*` → daily work

### 1.5 Repo Structure to Create

```
[project-name]/
├── app/
│   ├── app.py
│   ├── requirements.txt
│   └── Dockerfile
├── Jenkinsfile
├── terraform/
│   ├── main.tf
│   ├── variables.tf
│   └── outputs.tf
├── ansible/
│   ├── inventory
│   └── playbook.yml
├── k8s/
│   ├── deployment.yaml
│   └── service.yaml
├── monitoring/
│   ├── prometheus.yml
│   └── grafana-dashboard.json
└── README.md
```

---

## Phase 2 — Jenkins CI/CD

### 2.1 Install Jenkins

```bash
# Option A: Docker (recommended for local)
docker run -d \
  --name jenkins \
  -p 8080:8080 -p 50000:50000 \
  -v jenkins_home:/var/jenkins_home \
  -v /var/run/docker.sock:/var/run/docker.sock \
  jenkins/jenkins:lts

# Get initial password
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
        IMAGE_NAME = "yourdockerhubuser/appname"
        IMAGE_TAG  = "${BUILD_NUMBER}"
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Build') {
            steps {
                sh 'pip install -r app/requirements.txt'
            }
        }

        stage('Test') {
            steps {
                sh 'python -m pytest app/tests/ -v'
            }
        }

        stage('Docker Build') {
            steps {
                sh "docker build -t ${IMAGE_NAME}:${IMAGE_TAG} ./app"
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
                        docker push ${IMAGE_NAME}:${IMAGE_TAG}
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
        always {
            sh 'docker logout'
        }
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

### 3.1 Terraform — What to Provision (Local/Minikube)

> [!note] For local Kubernetes, Terraform creates namespace + configures cluster access

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
variable "namespace" {
  default = "production"
}

variable "app_name" {
  default = "my-app"
}
```

```hcl
# terraform/outputs.tf
output "namespace" {
  value = kubernetes_namespace.app_ns.metadata[0].name
}
```

```bash
cd terraform
terraform init
terraform plan
terraform apply
```

### 3.2 Ansible — What to Configure

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
      register: kubectl_check

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

### 4.2 Deployment YAML

```yaml
# k8s/deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: my-app
  namespace: production
spec:
  replicas: 2
  selector:
    matchLabels:
      app: my-app
  template:
    metadata:
      labels:
        app: my-app
    spec:
      containers:
        - name: my-app
          image: yourdockerhubuser/appname:latest
          ports:
            - containerPort: 5000
          readinessProbe:
            httpGet:
              path: /
              port: 5000
            initialDelaySeconds: 5
            periodSeconds: 10
```

### 4.3 Service YAML

```yaml
# k8s/service.yaml
apiVersion: v1
kind: Service
metadata:
  name: my-app-svc
  namespace: production
spec:
  type: NodePort
  selector:
    app: my-app
  ports:
    - port: 5000
      targetPort: 5000
      nodePort: 30080
```

### 4.4 Verify Deploy

```bash
kubectl apply -f k8s/
kubectl get pods -n production
kubectl get svc -n production

# Access app (OrbStack exposes NodePort directly)
http://localhost:30080
```

### 4.5 Expected Output

```
NAME                      READY   STATUS    RESTARTS   AGE
my-app-xxxxxxxxx-xxxxx    1/1     Running   0          2m
my-app-xxxxxxxxx-yyyyy    1/1     Running   0          2m

NAME         TYPE       CLUSTER-IP     PORT(S)          AGE
my-app-svc   NodePort   10.96.xx.xxx   5000:30080/TCP   2m
```

---

## Phase 5 — Monitoring

### 5.1 Add /metrics to Flask App

```python
# In app.py
from prometheus_client import Counter, Histogram, generate_latest, CONTENT_TYPE_LATEST
import time

REQUEST_COUNT = Counter('http_requests_total', 'Total HTTP requests', ['method', 'endpoint', 'status'])
REQUEST_LATENCY = Histogram('http_request_duration_seconds', 'HTTP request latency')

@app.route('/metrics')
def metrics():
    return generate_latest(), 200, {'Content-Type': CONTENT_TYPE_LATEST}
```

Add to `requirements.txt`:
```
prometheus-client==0.20.0
```

### 5.2 Prometheus Config

```yaml
# monitoring/prometheus.yml
global:
  scrape_interval: 15s

scrape_configs:
  - job_name: 'my-app'
    static_configs:
      - targets: ['localhost:30080']
```

```bash
# Run Prometheus
prometheus --config.file=monitoring/prometheus.yml
# UI at http://localhost:9090
```

### 5.3 Grafana Setup

```bash
# Run Grafana
docker run -d -p 3000:3000 grafana/grafana
# UI at http://localhost:3000 (admin/admin)
```

1. Add datasource: **Prometheus** → `http://localhost:9090`
2. Create dashboard with 4 panels:

| Panel | PromQL |
|-------|--------|
| Request Rate | `rate(http_requests_total[1m])` |
| Error Rate | `rate(http_requests_total{status=~"5.."}[1m])` |
| Latency p95 | `histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))` |
| Pod Health | `up{job="my-app"}` |

3. **Export dashboard** → save JSON → `monitoring/grafana-dashboard.json`

---

## README Completion

Run this to find all placeholders remaining:

```bash
grep -n "\[" README.md
```

Replace each:

| Placeholder | Replace with |
|-------------|-------------|
| `[ชื่อโปรเจค]` | actual project name |
| `[username]` | GitHub username |
| `[project-name]` | repo folder name |
| `[app-name]` | Kubernetes deployment name |
| `[namespace]` | `production` |
| `[resource]` | actual endpoint name (e.g. `notes`) |
| `XXXXX` in NodePort | `30080` |
| `[ชื่อ นามสกุล]` | real team member names + IDs |

---

## Final Deliverables

> [!todo] Before submission — verify all checked

### Files in Repo
- [ ] `app/app.py` — working Flask/Express app with `/metrics`
- [ ] `app/Dockerfile` — builds successfully
- [ ] `app/requirements.txt` — all deps pinned
- [ ] `Jenkinsfile` — all 6 stages defined
- [ ] `terraform/main.tf` + `variables.tf` + `outputs.tf`
- [ ] `ansible/inventory` + `playbook.yml`
- [ ] `k8s/deployment.yaml` + `service.yaml`
- [ ] `monitoring/prometheus.yml` + `grafana-dashboard.json`
- [ ] `README.md` — zero `[placeholder]` remaining

### Pipeline Works
- [ ] Push to `feature/*` → merge to `dev` → merge to `main` triggers Jenkins
- [ ] All 6 Jenkins stages pass (green)
- [ ] Docker image pushed to Docker Hub
- [ ] Pods running in Kubernetes (`STATUS: Running`)
- [ ] App accessible at `http://localhost:30080`
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
# Look at Events section
```

**Jenkins Docker Build fails**
```bash
sudo usermod -aG docker jenkins
sudo systemctl restart jenkins
```

**Prometheus target `DOWN`**
```bash
curl http://localhost:30080/metrics
# If empty → /metrics not implemented in app
```

**Terraform apply fails (namespace exists)**
```bash
terraform import kubernetes_namespace.app_ns production
```

---

## Team Task Split (Suggested)

| Member | Owns |
|--------|------|
| Member 1 | App code (Phase 1) + README |
| Member 2 | Jenkins + Dockerfile (Phase 2) |
| Member 3 | Terraform + Ansible (Phase 3) |
| Member 4 | Kubernetes + Monitoring (Phase 4–5) |

> [!warning] Everyone must understand the full pipeline for the demo — not just their own part.
