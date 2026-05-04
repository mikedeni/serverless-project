---
title: MyBrick — Commands Cheatsheet
tags:
  - cheatsheet
  - commands
  - reference
course: ENG23 3074
---

# ⚡ Commands Cheatsheet

> Every useful command in one place. See also [[PIPELINE_CHECKLIST]].

---

## Git

```bash
git clone https://github.com/mikedeni/serverless-project.git
git checkout -b feature/my-feature
git add .
git commit -m "feat: description"
git push origin main

# Trigger Jenkins without code change
git commit --allow-empty -m "ci: trigger pipeline"
git push origin main
```

---

## Docker Compose

```bash
# Start all services
cd app && docker compose up -d

# Stop
docker compose down

# Stop + delete volumes (wipe DB)
docker compose down -v

# Restart single service
docker compose restart backend

# View logs
docker compose logs backend --follow
docker compose logs db --follow

# Check status
docker compose ps
```

---

## Docker Images

```bash
# Build
docker build -t mikedeni/mybrick-backend:latest ./app/backend
docker build -t mikedeni/mybrick-frontend:latest ./app/frontend

# Push
docker login
docker push mikedeni/mybrick-backend:latest
docker push mikedeni/mybrick-frontend:latest

# List local images
docker images | grep mybrick
```

---

## .NET

```bash
# Run backend
cd app/backend && dotnet run

# Run tests
cd app && dotnet test backend.tests/ConstructionSaaS.Tests.csproj -v n

# Restore packages
dotnet restore

# Build release
dotnet build -c Release
```

---

## Node / Frontend

```bash
cd app/frontend
npm install
npm run dev       # dev server → http://localhost:5173
npm run build     # production build → dist/
```

---

## Terraform

```bash
cd terraform
terraform init
terraform plan
terraform apply
terraform apply -auto-approve

# Import existing namespace
terraform import kubernetes_namespace.mybrick production

# Destroy
terraform destroy
```

---

## Ansible

```bash
cd ansible
ansible-playbook -i inventory playbook.yml

# Dry run
ansible-playbook -i inventory playbook.yml --check

# Verbose
ansible-playbook -i inventory playbook.yml -v
```

---

## Kubernetes

```bash
# Context
kubectl config use-context orbstack
kubectl config current-context

# Namespace
kubectl create namespace production
kubectl get namespace production

# Deploy
kubectl apply -f k8s/
kubectl delete -f k8s/

# Monitor
kubectl get pods -n production
kubectl get pods -n production --watch
kubectl get svc  -n production
kubectl get all  -n production

# Debug
kubectl describe pod <pod-name> -n production
kubectl logs <pod-name> -n production
kubectl logs <pod-name> -n production --follow

# Rollout
kubectl rollout status deployment/mybrick-backend -n production
kubectl rollout restart deployment/mybrick-backend -n production
```

---

## Prometheus

```bash
# Check metrics endpoint
curl http://localhost:5154/metrics
curl http://localhost:30154/metrics

# Useful PromQL queries
rate(http_requests_received_total[1m])
rate(http_requests_received_total{code=~"5.."}[1m])
histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))
up{job="mybrick-backend"}
mybrick_projects_total
mybrick_expenses_total_baht
```

---

## Jenkins

```bash
# Run Jenkins
docker run -d \
  --name jenkins \
  -p 8080:8080 -p 50000:50000 \
  -v jenkins_home:/var/jenkins_home \
  -v /var/run/docker.sock:/var/run/docker.sock \
  jenkins/jenkins:lts

# Get initial password
docker exec jenkins cat /var/jenkins_home/secrets/initialAdminPassword

# Fix Docker access
sudo usermod -aG docker jenkins
sudo systemctl restart jenkins

# Restart Jenkins
docker restart jenkins
```

---

## Grafana

```bash
# Run Grafana
docker run -d -p 3000:3000 --name grafana grafana/grafana

# Restart
docker restart grafana
```

---

## Troubleshooting

```bash
# Port in use
lsof -i :5154
lsof -i :80
kill -9 <PID>

# Docker socket permission
sudo chmod 666 /var/run/docker.sock

# Reset everything (nuclear)
docker compose down -v
kubectl delete -f k8s/
kubectl delete namespace production
```
