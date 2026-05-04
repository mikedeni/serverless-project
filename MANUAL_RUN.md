---
title: MyBrick — Manual Run Guide
tags:
  - guide
  - local
  - setup
course: ENG23 3074
---

# 🛠️ MyBrick — Manual Run Guide

> วิธีรันโปรเจคด้วยตัวเองทีละขั้นตอน ไม่ผ่าน Jenkins pipeline

---

## Prerequisites Checklist

ติดตั้งครบก่อนเริ่ม:

- [ ] Docker Desktop / OrbStack (running)
- [ ] .NET SDK 8.0 → `dotnet --version`
- [ ] Node.js ≥ 20 → `node --version`
- [ ] MySQL 8 (ถ้ารันแบบไม่ใช้ Docker)

---

## Option A — Docker Compose (แนะนำ)

วิธีนี้รันทุก service พร้อมกันด้วยคำสั่งเดียว

### Step 1 — Clone repo

```bash
git clone https://github.com/mikedeni/serverless-project.git
cd serverless-project
```

### Step 2 — รัน Docker Compose

```bash
cd app
docker compose up -d
```

> รอประมาณ 30–60 วินาทีให้ DB initialize เสร็จ

### Step 3 — ตรวจสอบ services

```bash
docker compose ps
```

ควรเห็น STATUS `running` ทุกตัว:

```
NAME                STATUS
mybrick-db          running
mybrick-backend     running
mybrick-frontend    running
mybrick-prometheus  running
```

### Step 4 — เปิด browser

| Service | URL |
|---------|-----|
| Frontend | http://localhost:80 |
| Backend API | http://localhost:5154 |
| Swagger UI | http://localhost:5154/swagger |
| Prometheus metrics | http://localhost:5154/metrics |
| Prometheus UI | http://localhost:9090 |

### Step 5 — หยุด services

```bash
docker compose down
```

หยุดพร้อมลบ volume (DB data):

```bash
docker compose down -v
```

---

## Option B — รันแยกทีละ Service (dev mode)

ใช้เมื่อต้องการ hot-reload ระหว่างพัฒนา

### Step 1 — Start MySQL

```bash
cd app
docker compose up db -d
```

รอให้ DB พร้อม:

```bash
docker compose logs db --follow
# หยุดเมื่อเห็น: ready for connections
```

### Step 2 — รัน Backend

```bash
cd app/backend
dotnet run
```

- API: http://localhost:5154
- Swagger: http://localhost:5154/swagger
- Metrics: http://localhost:5154/metrics

> Connection string ใน `appsettings.json` ชี้ไปที่ `localhost:3306` อยู่แล้ว

### Step 3 — รัน Frontend

เปิด terminal ใหม่:

```bash
cd app/frontend
npm install
npm run dev
```

- Frontend: http://localhost:5173
- Proxy `/api` → ชี้ไปที่ backend `:5154` อัตโนมัติ (ดู `vite.config.js`)

### Step 4 — รัน Tests

```bash
cd app
dotnet test backend.tests/ConstructionSaaS.Tests.csproj -v n
```

---

## Option C — Build Docker Images ด้วยตัวเอง

ใช้เมื่อต้องการทดสอบ Dockerfile ก่อน push ขึ้น Hub

### Build Backend

```bash
cd app/backend
docker build -t mikedeni/mybrick-backend:latest .
```

### Build Frontend

```bash
cd app/frontend
docker build -t mikedeni/mybrick-frontend:latest .
```

### รัน image ที่ build

```bash
docker run -p 5154:5154 \
  -e "ConnectionStrings__DefaultConnection=Server=host.docker.internal;Database=ConstructionSaaS;User=mybrick_user;Password=MyBrick@2026;" \
  mikedeni/mybrick-backend:latest

docker run -p 80:80 mikedeni/mybrick-frontend:latest
```

### Push ขึ้น Docker Hub

```bash
docker login
docker push mikedeni/mybrick-backend:latest
docker push mikedeni/mybrick-frontend:latest
```

---

## Monitoring — Prometheus + Grafana

### Prometheus

Prometheus รันอยู่ใน Docker Compose แล้ว (port 9090)

```bash
# เปิดดู targets
open http://localhost:9090/targets
# ควรเห็น mybrick-backend → UP
```

### Grafana

```bash
docker run -d -p 3000:3000 --name grafana grafana/grafana
```

1. เปิด http://localhost:3000 (admin / admin)
2. **Connections → Data sources → Add → Prometheus**
   - URL: `http://host.docker.internal:9090`
   - Save & Test
3. **Dashboards → Import → Upload JSON file**
   - เลือกไฟล์ `monitoring/grafana-dashboard.json`
4. เห็น dashboard MyBrick — API Dashboard

---

## Kubernetes — Manual Deploy (OrbStack)

### Step 1 — ตั้งค่า context

```bash
kubectl config use-context orbstack
kubectl get nodes
```

### Step 2 — สร้าง namespace

```bash
kubectl create namespace production
```

### Step 3 — Deploy

```bash
kubectl apply -f k8s/
```

### Step 4 — ตรวจสอบ

```bash
kubectl get pods -n production
kubectl get svc  -n production
```

ผลลัพธ์ที่ควรได้:

```
NAME                                READY   STATUS    RESTARTS   AGE
mybrick-backend-xxxxxxxxx-xxxxx     1/1     Running   0          2m
mybrick-backend-xxxxxxxxx-yyyyy     1/1     Running   0          2m
mybrick-frontend-xxxxxxxxx-aaaaa    1/1     Running   0          2m
mybrick-frontend-xxxxxxxxx-bbbbb    1/1     Running   0          2m
```

### Step 5 — เปิด browser

| Service | URL |
|---------|-----|
| Frontend | http://localhost:30080 |
| Backend | http://localhost:30154 |
| Metrics | http://localhost:30154/metrics |

### Step 6 — ลบ resources

```bash
kubectl delete -f k8s/
kubectl delete namespace production
```

---

## Troubleshooting

**Docker Compose — backend ไม่ start (DB ยังไม่พร้อม)**

```bash
docker compose restart backend
```

**Frontend แสดง blank page**

```bash
docker compose logs frontend
# ตรวจดู nginx error
```

**dotnet run — port ถูกใช้อยู่**

```bash
lsof -i :5154
kill -9 <PID>
```

**MySQL connection refused**

```bash
docker compose ps db
# ถ้า STATUS ไม่ใช่ running
docker compose up db -d
```

**Kubernetes — ImagePullBackOff**

```bash
kubectl describe pod <pod-name> -n production
# image mikedeni/mybrick-* ต้องเป็น public บน Docker Hub
```
