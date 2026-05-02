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

## References

- [Markdown Guide](https://markdownguide.org)
- [GitHub Docs](https://docs.github.com)
