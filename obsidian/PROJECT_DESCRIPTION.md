---
title: ENG23 3074 — Serverless and Cloud Architectures
course: ENG23 3074
instructor: Dr. Nuntawut Kaoungku (AFHEA)
tags:
  - devops
  - cloud
  - serverless
  - kubernetes
  - jenkins
  - terraform
  - docker
  - prometheus
  - grafana
---

# ENG23 3074 — Serverless and Cloud Architectures

> [!info] Instructor
> **Dr. Nuntawut Kaoungku (AFHEA)**
> Assistant Professor of Computer Engineering

## Pipeline Overview

```
Git → Jenkins → Docker → Terraform/Ansible → Kubernetes → Grafana/Prometheus
```

---

## Learning Objectives

| Area | Focus |
|------|-------|
| **Version Control** | Git branching strategy & commit-driven automation via webhooks |
| **CI/CD (Jenkins)** | Declarative Jenkinsfile pipelines: build, test, package, deploy |
| **Containerization** | Dockerize apps, build images, push to Docker Hub |
| **Orchestration** | Deploy & manage containers using Pods, Deployments, Services |
| **IaC** | Provision with Terraform HCL; configure with Ansible playbooks |
| **Monitoring** | Scrape metrics, visualize dashboards, set up health alerts |

---

## Phased Approach

> [!tip] Each phase builds on the previous — code changes in Git ripple through the entire pipeline automatically.

- [[Phase 1 — Local Dev & Git]]
- [[Phase 2 — Jenkins CI/CD]]
- [[Phase 3 — Terraform + Ansible]]
- [[Phase 4 — Kubernetes Deploy]]
- [[Phase 5 — Prometheus + Grafana]]

---

## Phase 1 — Local Development & Git

### Application
- Simple web app or API (e.g., To-Do, Note Taker)
- Languages: Python/Flask, Node.js/Express

### Git Setup
- Create GitHub/GitLab repo
- Branching strategy: `main`, `dev`, `feature/*`
- Commit organized source code

### Repo Structure

```
app/
Dockerfile
Jenkinsfile
terraform/
ansible/
k8s/
monitoring/
README.md
```

---

## Phase 2 — Jenkins CI/CD

### Pipeline Stages

```
Checkout → Build → Test → Docker Build → Push Hub → Deploy
```

### Setup Tasks

- [ ] Install Jenkins (local or Docker) with required plugins
- [ ] Create `Jenkinsfile` (declarative) in repo root
- [ ] Trigger pipeline via webhook on every push
- [ ] Build Docker image using Dockerfile
- [ ] Push image to Docker Hub (credentials required)
- [ ] Deploy using Terraform and Ansible

---

## Phase 3 — Infrastructure as Code

### Terraform

```hcl
# terraform init → plan → apply
```

Files: `main.tf`, `variables.tf`, `outputs.tf`, `terraform.tfstate`

### Ansible

```
inventory       # localhost or remote hosts
playbook.yml    # setup tasks
roles/          # reusable configs
```

> [!note] Integration
> Jenkins runs `terraform apply` then `ansible-playbook` in the Deploy stage.

---

## Phase 4 — Kubernetes

### Core Resources

| Resource | Purpose | Key Commands |
|----------|---------|--------------|
| **Pod** | Smallest deployable unit | `kubectl get pods` · `kubectl describe pod <name>` |
| **Deployment** | Manages replicas & updates | `kubectl apply -f deployment.yaml` · `kubectl rollout status deploy/app` |
| **Service** | Exposes Pods (ClusterIP / NodePort) | `kubectl apply -f service.yaml` · `kubectl get svc` |
| **Namespace** | Logical separation (dev/staging/prod) | `kubectl create ns production` · `kubectl -n production get all` |

### Expected Output
```
NAME                        READY   STATUS    RESTARTS   AGE
[app-name]-xxxxxxxxx-xxxxx  1/1     Running   0          2m
[app-name]-xxxxxxxxx-yyyyy  1/1     Running   0          2m

NAME            TYPE       CLUSTER-IP     PORT(S)          AGE
[app-name]-svc  NodePort   10.96.xx.xxx   5000:30080/TCP   2m
```

Access app: `http://localhost:30080`

> [!note]
> Kubernetes YAML files stored in repo and applied via Jenkins.

---

## Phase 5 — Monitoring

### Prometheus
- Expose `/metrics` endpoint
- Configure `prometheus.yml`
- Track: request count, latency, errors, uptime

### Grafana
- Connect to Prometheus datasource
- Create dashboards: health, request rate, latency
- Set alert rules (e.g., error rate > 5%)
- Export dashboard JSON to repo

---

## End-to-End Flow

```
Developer
  → Commit + Push
  → GitHub Webhook
  → Jenkins: Build + Test
  → Docker: Build + Push Image
  → Terraform/Ansible: Provision + Configure
  → Kubernetes: Deploy Pods
  → Running App
        ↑
  Prometheus (metrics) → Grafana (dashboards)
```

---

## Deliverables

- [ ] **Git Repository** — `app/`, `terraform/`, `ansible/`, `k8s/`, `monitoring/`, `README.md`
- [ ] **Jenkinsfile** — Checkout → Build → Docker → Push → Deploy
- [ ] **Terraform** — `main.tf`, `variables.tf`, `outputs.tf`
- [ ] **Ansible** — `playbook.yml`, `inventory`
- [ ] **Kubernetes** — `deployment.yaml`, `service.yaml`
- [ ] **Monitoring** — `prometheus.yml`, Grafana dashboard JSON
- [ ] **Presentation/Demo** — live demo + architecture diagram

---

## Assignment Steps

> [!todo] Steps
> 1. **Form Groups** — 3–4 students per team
> 2. **Propose Project** — simple app + full pipeline description
> 3. **Architecture Diagram** — complete pipeline and integrations
> 4. **Submit GitHub + README.md** — setup, pipeline, team info

### Example System
**Note API** — Python Flask · MySQL · Docker · Jenkins · Terraform · Ansible · Kubernetes · Prometheus · Grafana

---

## Branching Strategy

```
main        — production-ready, protected
dev         — integration branch before merge to main
feature/*   — individual feature development (e.g. feature/add-login)
```

| Branch | Protected | Trigger |
|--------|-----------|---------|
| `main` | ✅ | auto pipeline on merge |
| `dev` | ✅ | test before merge to main |
| `feature/*` | ❌ | merge into dev when done |

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/` | Health check |
| `GET` | `/metrics` | Prometheus metrics |
| `GET` | `/[resource]` | List resources |
| `POST` | `/[resource]` | Create resource |
| `DELETE` | `/[resource]/:id` | Delete resource |

---

## Monitoring Metrics (PromQL)

| Panel | PromQL | Shows |
|-------|--------|-------|
| Request Rate | `rate(http_requests_total[1m])` | requests/sec |
| Error Rate | `rate(http_requests_total{status=~"5.."}[1m])` | 5xx errors/sec |
| Latency p95 | `histogram_quantile(0.95, ...)` | 95th percentile response time |
| Pod Health | `up{job="[app-name]"}` | service up/down (1/0) |

---

## Troubleshooting

**Pods stuck at `Pending`**
```bash
kubectl describe pod [pod-name] -n [namespace]
# Check Events: resource limits or image pull error
```

**Jenkins pipeline fails at Docker Build**
```bash
sudo systemctl start docker
sudo usermod -aG docker jenkins
```

**Prometheus target shows DOWN**
```bash
curl http://localhost:5000/metrics
# Verify host:port in prometheus.yml matches app
```

---

## References

- [Markdown Guide](https://markdownguide.org)
- [GitHub Docs](https://docs.github.com)

---

## 🔗 Related

- [[HOME]] — Vault index
- [[PROJECT_GUIDE]] — How to complete each phase
- [[INSTALL_GUIDE]] — Setup all deliverables
- [[PIPELINE_CHECKLIST]] — Verify all deliverables done
- [[DEMO_GUIDE]] — Present the project
