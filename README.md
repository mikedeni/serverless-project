# 🚀 MyBrick — ENG23 3074

> ระบบบริหารงานก่อสร้างแบบครบวงจร สร้างด้วย .NET 8 Web API + React/Vite containerize ด้วย Docker และ deploy บน Kubernetes ผ่าน Jenkins pipeline แบบอัตโนมัติ

---

## 👥 สมาชิกในกลุ่ม

| รหัสนักศึกษา | ชื่อ-นามสกุล | ความรับผิดชอบ |
|-------------|-------------|---------------|
| 6XXXXXXX | ชื่อ นามสกุล | Git, App Development |
| 6XXXXXXX | ชื่อ นามสกุล | Jenkins, Docker |
| 6XXXXXXX | ชื่อ นามสกุล | Terraform, Ansible |
| 6XXXXXXX | ชื่อ นามสกุล | Kubernetes, Monitoring |

---

## 📌 ภาพรวมโปรเจค

### แอปพลิเคชัน
- **ชื่อ:** MyBrick
- **ประเภท:** REST API + Web Application
- **Backend:** .NET 8 ASP.NET Core Web API + Dapper + MySQL 8
- **Frontend:** React 18 + Vite + Axios
- **คำอธิบาย:** ระบบบริหารงานก่อสร้างสำหรับบริษัทรับเหมา รองรับการจัดการโปรเจค, คนงาน, วัสดุ, ค่าใช้จ่าย, ใบเสนอราคา และรายงาน พร้อม Prometheus metrics

### Architecture Diagram

```
Developer
    │
    ▼  git push
 GitHub ──── webhook ────▶ Jenkins CI/CD
                                │
                    ┌───────────┼───────────┐
                    ▼           ▼           ▼
                 Build        Test      Docker Build
              (dotnet)      (xUnit)  (backend+frontend)
                                            │
                                            ▼
                                       Docker Hub
                                            │
                                    ┌───────┴───────┐
                                    ▼               ▼
                                Terraform        Ansible
                             (namespace)    (kubectl config)
                                    │               │
                                    └───────┬───────┘
                                            ▼
                                   Kubernetes Cluster (OrbStack)
                              ┌─────────────────────────────┐
                              │  namespace: production       │
                              │                             │
                              │  mybrick-backend  x2 pods  │
                              │  mybrick-frontend x2 pods  │
                              │                             │
                              │  backend-svc  :30154        │
                              │  frontend-svc :30080        │
                              └─────────────────────────────┘
                                            │
                              ┌─────────────┴──────────────┐
                              ▼                             ▼
                          Prometheus  ──────────────▶  Grafana
                     (scrape :30154/metrics)         (dashboard)
```

---

## 📁 โครงสร้าง Repository

```
serverless-project/
├── app/
│   ├── backend/                  # .NET 8 ASP.NET Core Web API
│   │   ├── Controllers/          # REST endpoints
│   │   ├── Services/             # Business logic
│   │   ├── Repositories/         # Dapper data access
│   │   ├── Models/               # Domain models
│   │   ├── DTOs/                 # Request/response shapes
│   │   ├── Data/                 # DapperContext
│   │   ├── Monitoring/           # BusinessMetrics (prometheus-net)
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── Dockerfile
│   ├── backend.tests/            # xUnit test project
│   │   ├── ProjectServiceTests.cs
│   │   ├── DashboardServiceTests.cs
│   │   └── ReportServiceTests.cs
│   ├── frontend/                 # React + Vite
│   │   ├── src/
│   │   │   ├── pages/
│   │   │   ├── components/
│   │   │   ├── contexts/         # AuthContext (JWT)
│   │   │   └── utils/api.js      # Axios wrapper
│   │   └── Dockerfile
│   ├── database/
│   │   ├── schema.sql            # Full DDL
│   │   └── update.sql            # Migration patches
│   ├── docker-compose.yml
│   └── prometheus.yml
├── Jenkinsfile
├── terraform/
│   ├── main.tf
│   ├── variables.tf
│   └── outputs.tf
├── ansible/
│   ├── inventory
│   └── playbook.yml
├── k8s/
│   ├── backend-deployment.yaml
│   ├── frontend-deployment.yaml
│   └── services.yaml
├── monitoring/
│   └── grafana-dashboard.json
└── README.md
```

---

## ⚙️ สิ่งที่ต้องติดตั้งก่อน (Prerequisites)

| Tool | Version | หน้าที่ |
|------|---------|---------|
| Git | ≥ 2.x | จัดการ source code |
| Docker | ≥ 24.x | สร้างและรัน container |
| .NET SDK | 8.0 | build และ test backend |
| Node.js | ≥ 20.x | build frontend |
| Jenkins | ≥ 2.4xx | CI/CD automation |
| Terraform | ≥ 1.x | Provision infrastructure |
| Ansible | ≥ 2.15 | Configure environment |
| kubectl | ≥ 1.28 | สั่งงาน Kubernetes |
| OrbStack / Minikube | latest | Kubernetes local |
| Prometheus | ≥ 2.x | เก็บ metrics |
| Grafana | ≥ 10.x | แสดง dashboard |

---

## 🏃 วิธีรันโปรเจค (Quick Start)

### 1. Clone Repository

```bash
git clone https://github.com/mikedeni/serverless-project.git
cd serverless-project
```

### 2. รันด้วย Docker Compose (ทั้งระบบ)

```bash
cd app
docker compose up -d
```

| Service | URL |
|---------|-----|
| Frontend | http://localhost:80 |
| Backend API | http://localhost:5154 |
| Swagger | http://localhost:5154/swagger |
| Metrics | http://localhost:5154/metrics |
| Prometheus | http://localhost:9090 |

### 3. รัน Backend โดยตรง (dev)

```bash
cd app/backend
dotnet run
```

### 4. รัน Frontend โดยตรง (dev)

```bash
cd app/frontend
npm install
npm run dev
# http://localhost:5173
```

### 5. รัน Tests

```bash
cd app
dotnet test backend.tests/ConstructionSaaS.Tests.csproj -v n
```

---

## 🔄 CI/CD Pipeline (Jenkins)

### ลำดับการทำงาน

```
Checkout ──▶ Test (xUnit) ──▶ Docker Build ──▶ Push to Hub ──▶ Deploy
```

| Stage | คำอธิบาย |
|-------|----------|
| **Checkout** | ดึงโค้ดล่าสุดจาก GitHub |
| **Test** | รัน xUnit tests สำหรับ backend |
| **Docker Build** | สร้าง image mybrick-backend + mybrick-frontend |
| **Push to Hub** | อัปโหลด image ขึ้น Docker Hub |
| **Deploy** | Terraform + Ansible + kubectl apply |

### วิธีตั้งค่า Jenkins

1. ติดตั้ง Jenkins และเปิดที่ `http://localhost:8080`
2. ติดตั้ง plugin: **Git**, **Pipeline**, **Docker Pipeline**
3. เพิ่ม credentials ID `dockerhub-credentials` (Username/Password)
4. สร้าง Pipeline job ชี้ไปที่ repository นี้
5. ตั้งค่า Webhook ใน GitHub:
   - **Settings → Webhooks → Add webhook**
   - Payload URL: `http://[jenkins-host]:8080/github-webhook/`
   - Content type: `application/json`
   - Trigger: **Just the push event**

---

## 🏗️ Infrastructure as Code

### Terraform

```bash
cd terraform
terraform init
terraform plan
terraform apply
```

สิ่งที่ Terraform สร้าง: Kubernetes namespace `production`

### Ansible

```bash
cd ansible
ansible-playbook -i inventory playbook.yml
```

สิ่งที่ Ansible ทำ: ตรวจสอบ kubectl, ตั้งค่า context `orbstack`, สร้าง namespace ถ้าไม่มี

---

## ☸️ Kubernetes Deployment

### Apply Manifests

```bash
kubectl apply -f k8s/
```

### ตรวจสอบสถานะ

```bash
kubectl get pods -n production
kubectl get svc  -n production
```

### ผลลัพธ์ที่ควรได้

```
NAME                                READY   STATUS    RESTARTS   AGE
mybrick-backend-xxxxxxxxx-xxxxx     1/1     Running   0          2m
mybrick-backend-xxxxxxxxx-yyyyy     1/1     Running   0          2m
mybrick-frontend-xxxxxxxxx-aaaaa    1/1     Running   0          2m
mybrick-frontend-xxxxxxxxx-bbbbb    1/1     Running   0          2m

NAME                   TYPE       CLUSTER-IP     PORT(S)          AGE
mybrick-backend-svc    NodePort   10.96.xx.xxx   5154:30154/TCP   2m
mybrick-frontend-svc   NodePort   10.96.xx.yyy   80:30080/TCP     2m
```

### เข้าถึงแอปพลิเคชัน (via Kubernetes)

```
Frontend : http://localhost:30080
Backend  : http://localhost:30154
Metrics  : http://localhost:30154/metrics
```

---

## 📊 Monitoring

### Prometheus

Scrape config อยู่ใน `app/prometheus.yml` — ดึง metrics จาก backend ทุก 15 วินาที

```bash
# รันผ่าน Docker Compose (อยู่ใน app/docker-compose.yml แล้ว)
cd app && docker compose up prometheus
# UI: http://localhost:9090
```

### Grafana

```bash
docker run -d -p 3000:3000 grafana/grafana
# UI: http://localhost:3000 (admin/admin)
```

1. Add datasource: **Prometheus** → `http://localhost:9090`
2. Import dashboard: **Dashboards → Import** → อัปโหลด `monitoring/grafana-dashboard.json`

### Panels ใน Dashboard

| Panel | PromQL | แสดงข้อมูล |
|-------|--------|-----------|
| Request Rate | `rate(http_requests_received_total[1m])` | req/s |
| Error Rate | `rate(http_requests_received_total{code=~"5.."}[1m])` | 5xx/s |
| Latency p95 | `histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))` | response time |
| Pod Health | `up{job="mybrick-backend"}` | up/down |
| Total Projects | `mybrick_projects_total` | จำนวนโปรเจค |
| Total Expenses | `mybrick_expenses_total_baht` | ค่าใช้จ่าย (THB) |

---

## 🌿 Branching Strategy

```
main        — production-ready, protected
dev         — integration ก่อน merge ขึ้น main
feature/*   — พัฒนา feature แยกกัน
```

| Branch | Protected | Trigger |
|--------|-----------|---------|
| `main` | ✅ | auto pipeline on merge |
| `dev` | ✅ | ทดสอบก่อน merge ขึ้น main |
| `feature/*` | ❌ | merge เข้า dev เมื่อเสร็จ |

---

## 🧪 API Endpoints

| Method | Endpoint | คำอธิบาย |
|--------|----------|----------|
| `GET` | `/metrics` | Prometheus metrics |
| `POST` | `/api/auth/login` | เข้าสู่ระบบ |
| `POST` | `/api/auth/register` | สมัครสมาชิก |
| `GET/POST` | `/api/projects` | รายการ / สร้างโปรเจค |
| `GET/POST` | `/api/workers` | รายการ / เพิ่มคนงาน |
| `GET/POST` | `/api/materials` | รายการ / เพิ่มวัสดุ |
| `GET/POST` | `/api/tasks` | รายการ / สร้างงาน |
| `GET/POST` | `/api/expenses` | รายการ / เพิ่มค่าใช้จ่าย |
| `GET/POST` | `/api/invoices` | รายการ / สร้างใบแจ้งหนี้ |
| `GET/POST` | `/api/quotations` | รายการ / สร้างใบเสนอราคา |
| `GET` | `/api/dashboard` | ภาพรวม dashboard |
| `GET` | `/api/reports/*` | รายงานต่าง ๆ |

---

## 🐛 ปัญหาที่พบบ่อย (Troubleshooting)

**Pods ค้างอยู่ที่ `Pending`**
```bash
kubectl describe pod [pod-name] -n production
# ดูที่ Events section
```

**Backend ต่อ MySQL ไม่ได้**
```bash
kubectl get pods -n production | grep db
# ตรวจ connection string ใน env ให้ตรงกับ service name
```

**Jenkins Docker Build ล้มเหลว**
```bash
sudo usermod -aG docker jenkins
sudo systemctl restart jenkins
```

**Prometheus target แสดง DOWN**
```bash
curl http://localhost:30154/metrics
# ถ้าไม่มีข้อมูล — ตรวจว่า prometheus-net middleware ถูก register ใน Program.cs
```

**Terraform apply ล้มเหลว (namespace มีอยู่แล้ว)**
```bash
terraform import kubernetes_namespace.mybrick production
```

---

## 📚 เอกสารอ้างอิง

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [React Documentation](https://react.dev)
- [Jenkins Pipeline Syntax](https://www.jenkins.io/doc/book/pipeline/syntax/)
- [Terraform Documentation](https://developer.hashicorp.com/terraform/docs)
- [Ansible Documentation](https://docs.ansible.com/)
- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [Prometheus Documentation](https://prometheus.io/docs/)
- [Grafana Documentation](https://grafana.com/docs/)
- [prometheus-net](https://github.com/prometheus-net/prometheus-net)
