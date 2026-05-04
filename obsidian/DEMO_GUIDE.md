---
title: MyBrick — Demo Guide
tags:
  - guide
  - demo
  - presentation
course: ENG23 3074
---

# 🎯 MyBrick — Demo Guide

> What to prepare and how to run the live demo for ENG23 3074 presentation.

---

## Pre-Demo Checklist

Complete before walking into the room:

### Infrastructure
- [ ] OrbStack running → `kubectl get nodes` shows `Ready`
- [ ] Docker Desktop running
- [ ] Jenkins running at http://localhost:8080
- [ ] `dockerhub-credentials` set in Jenkins
- [ ] GitHub webhook configured and active
- [ ] Docker Hub repos `mikedeni/mybrick-backend` + `mikedeni/mybrick-frontend` → **Public**

### App
- [ ] `cd app && docker compose up -d` → all 4 containers `running`
- [ ] http://localhost:80 loads login page
- [ ] http://localhost:5154/metrics returns Prometheus data
- [ ] http://localhost:9090/targets → `mybrick-backend` = **UP**

### Monitoring
- [ ] Grafana running at http://localhost:3000
- [ ] Prometheus datasource connected
- [ ] `monitoring/grafana-dashboard.json` imported and showing data

### Kubernetes
- [ ] `kubectl get pods -n production` → all pods `Running`
- [ ] http://localhost:30080 loads frontend

---

## Live Demo Flow

### Step 1 — Show Architecture

Open README → Architecture Diagram section. Walk through:

```
Git push → Webhook → Jenkins → Docker Hub → Terraform/Ansible → Kubernetes → Prometheus → Grafana
```

### Step 2 — Trigger Pipeline

Make a small code change and push to `main`:

```bash
git commit --allow-empty -m "demo: trigger pipeline"
git push origin main
```

Open Jenkins http://localhost:8080 → watch pipeline auto-trigger.

### Step 3 — Show All 5 Stages Pass

Walk through each stage while it runs:

| Stage | What to say |
|-------|-------------|
| **Checkout** | Jenkins pulls latest code from GitHub via webhook |
| **Test** | xUnit tests run against backend services |
| **Docker Build** | Builds `mybrick-backend` and `mybrick-frontend` images |
| **Push to Hub** | Images pushed to `mikedeni/*` on Docker Hub |
| **Deploy** | Terraform provisions namespace → Ansible configures → kubectl applies k8s manifests |

### Step 4 — Show Docker Hub

Open https://hub.docker.com/r/mikedeni/mybrick-backend/tags → new build number tag appeared.

### Step 5 — Show Kubernetes

```bash
kubectl get pods -n production
kubectl get svc  -n production
```

All pods `Running`. Services expose NodePort 30080 (frontend) and 30154 (backend).

### Step 6 — Show Running App

Open http://localhost:30080 → login, navigate dashboard.

Hit an API endpoint to generate traffic:

```bash
curl http://localhost:30154/metrics | head -20
```

### Step 7 — Show Prometheus

Open http://localhost:9090 → Status → Targets → `mybrick-backend` = **UP**

Run a query:

```
rate(http_requests_received_total[1m])
```

### Step 8 — Show Grafana Dashboard

Open http://localhost:3000 → MyBrick — API Dashboard

Point out each panel:
- **Request Rate** — live traffic
- **Error Rate** — 5xx errors
- **Latency p95** — response time
- **Pod Health** — service up/down
- **Total Projects** — business metric
- **Total Expenses (THB)** — business metric

---

## Each Member Explains Their Phase

| Member | Phase | Key points |
|--------|-------|-----------|
| Member 1 | App + Git | .NET 8 API, React frontend, branching strategy |
| Member 2 | Jenkins + Docker | Jenkinsfile stages, Docker Hub push, webhook trigger |
| Member 3 | Terraform + Ansible | Namespace provisioning, environment configuration |
| Member 4 | Kubernetes + Monitoring | Deployments, NodePort services, Prometheus scrape, Grafana |

---

## Critical Risks

Things most likely to fail — verify the day before:

| Risk | Prevention |
|------|-----------|
| Webhook doesn't trigger Jenkins | Test manually: push dummy commit, watch Jenkins |
| ImagePullBackOff on pods | Confirm Docker Hub repos are **Public** |
| Prometheus target DOWN | Run `curl localhost:5154/metrics` — must return data |
| DB not ready → backend crash | `docker compose restart backend` after DB starts |
| Jenkins no Docker access | `sudo usermod -aG docker jenkins && sudo systemctl restart jenkins` |

---

## Backup Plan

If pipeline breaks during demo:

```bash
# Skip Jenkins — deploy directly
cd app && docker compose up -d
kubectl apply -f k8s/
```

Show each component working independently — pipeline concept still demonstrated.

---

## 🔗 Related

- [[HOME]] — Vault index
- [[PIPELINE_CHECKLIST]] — Ordered checklist to verify before demo
- [[COMMANDS_CHEATSHEET]] — Quick command lookup
- [[INSTALL_GUIDE]] — Full setup if something isn't running
- [[PROJECT_GUIDE]] — Phase breakdown reference
