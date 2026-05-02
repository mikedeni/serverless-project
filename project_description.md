ENG23 3074 - Serverless and Cloud Architectures Project

ENG23 3074 — Serverless and Cloud Architectures

Project
Git  →  Jenkins  →  Docker  →  Terraform / Ansible  →  Kubernetes  →  Grafana / Prometheus

Dr. Nuntawut Kaoungku (AFHEA)
Assistant Professor of Computer Engineering


Introduction & Learning Objectives

Version Control
Git branching strategy & commit-driven automation via webhooks.

CI/CD (Jenkins)
Declarative Jenkinsfile pipelines: build, test, package, deploy.

Containerization (Docker)
Dockerize apps, build images, push to Docker Hub registry.

Orchestration (Kubernetes)
Deploy & manage containers using Pods, Deployments, and Services.

IaC (Terraform + Ansible)
Provision infra with Terraform HCL; configure with Ansible playbooks.

Monitoring (Prometheus/Grafana)
Scrape metrics, visualize dashboards, set up health alerts.


Project Scope & Phased Approach

Phase 1 — Local Dev & Git
Code, Dockerfile, branch strategy

Phase 2 — Jenkins CI/CD
Build → Test → Docker → Hub

Phase 3 — Terraform + Ansible
Provision infra & configure env

Phase 4 — Kubernetes Deploy
Pods, Services, Deployments

Phase 5 — Prometheus + Grafana
Scrape metrics, Dashboards

Each phase builds on the previous — code changes in Git ripple through the entire pipeline automatically.


Phase 1 — Local Development & Git

Choose Your Application
Simple web app or API (e.g., To-Do, Note Taker).
Languages: Python/Flask, Node.js/Express.

Set Up Git Repository
Create GitHub/GitLab repo.
Define branching strategy (main, dev, feature/*).
Commit organized source code.

Write Dockerfile
Containerize the app with a Dockerfile.
Ensure docker build and docker run work locally.

Repo Structure
app/
Dockerfile
Jenkinsfile
terraform/
ansible/
monitoring/
README.md


Phase 2 — Automation with Jenkins CI/CD

Pipeline Stages:

Checkout → Pull latest from Git
Build → Compile / install dependencies
Test → Unit & integration
Docker Build → Build Docker image
Push Hub → Push to Docker Hub
Deploy → Trigger Terraform/Ansible

Key Setup Tasks:

- Install Jenkins (local or Docker) with required plugins.
- Create Jenkinsfile (declarative) in repo root.
- Trigger pipeline via webhook on every push.
- Build Docker image using Dockerfile.
- Push image to Docker Hub (credentials required).
- Deploy using Terraform and Ansible.


Phase 3 — Infrastructure as Code: Terraform & Ansible

Terraform
- Write .tf files for infrastructure.
- Commands: terraform init → plan → apply
- Store terraform.tfstate locally.
- Use variables.tf and outputs.tf

Ansible
- Inventory file (localhost or remote)
- playbook.yml with setup tasks
- roles/ for reusable configs

Pipeline Integration:
Jenkins runs terraform apply then ansible-playbook in Deploy stage.


Phase 4 — Container Orchestration with Kubernetes

Pod
- Smallest deployable unit
- Runs one or more containers

Commands:
kubectl get pods
kubectl describe pod <name>

Deployment
- Manages replicas and updates

Commands:
kubectl apply -f deployment.yaml
kubectl rollout status deploy/app

Service
- Exposes Pods

Types:
ClusterIP (internal)
NodePort (external)

Commands:
kubectl apply -f service.yaml
kubectl get svc

Namespace
- Logical separation (dev, staging, prod)

Commands:
kubectl create ns production
kubectl -n production get all

Kubernetes YAML files stored in repo and applied via Jenkins.


Phase 5 — Monitoring with Prometheus & Grafana

Prometheus
- Expose /metrics endpoint
- Configure prometheus.yml
- Track request count, latency, errors, uptime

Grafana
- Connect to Prometheus
- Create dashboards (health, request rate, latency)
- Set alert rules (e.g., error rate > 5%)
- Export dashboard JSON to repo

Monitoring runs alongside application.


End-to-End Pipeline Overview

Developer → GitHub → Jenkins → Docker Hub → Terraform/Ansible → Kubernetes → Application

Flow:
Code + Commit
→ Push/Webhook
→ Build + Test
→ Build + Push Image
→ Provision + Configure
→ Deploy Pods
→ Running App

Prometheus → collects metrics
Grafana → visualizes dashboards

Monitoring layer is always active.


Example System — Note API
Python Flask · MySQL · Docker · Jenkins · Terraform · Ansible · Kubernetes · Prometheus · Grafana


Project Structure & Deliverables

Git Repository
- app/, terraform/, ansible/, k8s/, monitoring/
- Document branching in README.md

Jenkinsfile
- Pipeline: Checkout → Build → Docker → Push → Deploy

Terraform + Ansible
- main.tf, variables.tf, outputs.tf
- playbook.yml, inventory

Kubernetes
- deployment.yaml
- service.yaml

Monitoring
- prometheus.yml
- Grafana dashboard JSON

Presentation / Demo
- Live demo: git push → pipeline → running Pods
- Architecture diagram


Assignment

01 Form Groups
3–4 students per team

02 Propose Project
Simple app + full pipeline description

03 Architecture Diagram
Show complete pipeline and integrations

04 Submit GitHub + README.md
Include setup, pipeline, team info

References:
markdownguide.org
docs.github.com


Slides:
Slide 1
Slide 2
Slide 3
Slide 4
Slide 5
Slide 6
Slide 7
Slide 8
Slide 9
Slide 10
Slide 11
Slide 12
Slide 13