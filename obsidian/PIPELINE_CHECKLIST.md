---
title: MyBrick — Pipeline Checklist
tags:
  - checklist
  - pipeline
  - demo
course: ENG23 3074
---

# ✅ Pipeline Checklist

> Run in order. Each phase depends on the previous. See [[COMMANDS_CHEATSHEET]] for commands.

---

## Phase 0 — Prerequisites

- [ ] Docker running → `docker info`
- [ ] OrbStack running → `kubectl get nodes` shows `Ready`
- [ ] .NET 8 installed → `dotnet --version`
- [ ] Node 20+ installed → `node --version`
- [ ] Terraform installed → `terraform --version`
- [ ] Ansible installed → `ansible --version`

---

## Phase 1 — App (Local)

- [ ] `cd app && docker compose up -d`
- [ ] `docker compose ps` → all 4 containers `running`
- [ ] http://localhost:80 → frontend loads
- [ ] http://localhost:5154/swagger → Swagger UI loads
- [ ] http://localhost:5154/metrics → Prometheus data returned
- [ ] `dotnet test backend.tests/ConstructionSaaS.Tests.csproj -v n` → all pass

---

## Phase 2 — Jenkins

- [ ] Jenkins running at http://localhost:8080
- [ ] Plugin installed: Git, Pipeline, Docker Pipeline, GitHub Integration
- [ ] Credential `dockerhub-credentials` added
- [ ] Pipeline job created → points to `Jenkinsfile` in repo
- [ ] GitHub webhook configured → Payload URL set, push event enabled
- [ ] Test trigger: `git commit --allow-empty -m "ci: test" && git push origin main`
- [ ] All 5 stages pass ✅ (Checkout → Test → Docker Build → Push → Deploy)

---

## Phase 3 — Docker Hub

- [ ] `mikedeni/mybrick-backend` repo exists → **Public**
- [ ] `mikedeni/mybrick-frontend` repo exists → **Public**
- [ ] New image tag visible after Jenkins build

---

## Phase 4 — Terraform

- [ ] `kubectl config use-context orbstack`
- [ ] `cd terraform && terraform init`
- [ ] `terraform plan` → shows 1 resource to create
- [ ] `terraform apply` → success
- [ ] `kubectl get namespace production` → exists

---

## Phase 5 — Ansible

- [ ] `cd ansible && ansible-playbook -i inventory playbook.yml`
- [ ] All tasks → `ok` or `changed`, no failures
- [ ] kubectl context confirmed as `orbstack`

---

## Phase 6 — Kubernetes

- [ ] `kubectl apply -f k8s/`
- [ ] `kubectl get pods -n production` → all `Running` (4 pods)
- [ ] `kubectl get svc -n production` → backend `:30154`, frontend `:30080`
- [ ] http://localhost:30080 → frontend loads
- [ ] http://localhost:30154/metrics → returns data

---

## Phase 7 — Prometheus

- [ ] http://localhost:9090/targets → `mybrick-backend` = **UP**
- [ ] Query `rate(http_requests_received_total[1m])` → returns data
- [ ] Query `up{job="mybrick-backend"}` → returns `1`

---

## Phase 8 — Grafana

- [ ] Grafana running at http://localhost:3000
- [ ] Prometheus datasource added → **Data source is working**
- [ ] `monitoring/grafana-dashboard.json` imported
- [ ] All 6 panels show data (no "No data" panels)

---

## Demo Day Final Check

- [ ] All phases above ✅
- [ ] Dummy push triggers Jenkins automatically (webhook works)
- [ ] Each member knows their phase to explain
- [ ] Architecture diagram ready (README → ภาพรวมโปรเจค)
- [ ] Backup plan ready: `docker compose up -d` + `kubectl apply -f k8s/`
