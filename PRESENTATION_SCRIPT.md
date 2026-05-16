---
title: MyBrick — สคริปต์นำเสนอ 10 นาที (ตามเกณฑ์การให้คะแนน 100 คะแนน)
subtitle: 4 คน × 2.5 นาที | ครอบคลุมทุก Phase
---

# MyBrick — สคริปต์นำเสนอ 10 นาที

**รูปแบบ:** 4 คนพูด × 2.5 นาที  
**เป้าหมาย:** แสดงให้เห็นว่าระบบทำงานครบทุก Phase ตามเกณฑ์

---

## ภาพรวม — ใครพูดอะไร

| ผู้พูด | Phase ที่รับผิดชอบ | คะแนน |
|--------|-------------------|-------|
| ผู้พูด 1 (ภูผา) | Phase 1: Git & Source Code | 10 คะแนน |
| ผู้พูด 2 (พิชญ์สินี) | Phase 2: Jenkins CI/CD + Docker | 25 คะแนน |
| ผู้พูด 3 (โกวิท) | Phase 3: Terraform + Ansible + Phase 4: Kubernetes | 40 คะแนน |
| ผู้พูด 4 (ตะวัน) | Phase 5: Prometheus + Grafana + Live Demo | 25+10 คะแนน |

---

# ผู้พูด 1: Git & Source Code (0:00–2:30)
**Phase 1 — 10 คะแนน**

---

> "สวัสดีครับ ผมชื่อภูผา วันนี้พวกเราจะนำเสนอโปรเจค MyBrick ซึ่งเป็นระบบบริหารงานก่อสร้าง ผมจะเริ่มจาก Phase แรกก่อนเลย คือเรื่อง Git และ Source Code ครับ"

---

### 1.1 โครงสร้าง Repository และ Branching Strategy (3 คะแนน)

> "โปรเจคนี้เก็บอยู่บน GitHub ครับ โครงสร้างหลักแบ่งเป็น 5 ส่วน"

```
serverless-project/
├── app/          → ตัวแอปหลัก (backend + frontend + database)
├── k8s/          → ไฟล์ Kubernetes manifest
├── terraform/    → สร้าง infrastructure
├── ansible/      → configure เครื่อง
└── Jenkinsfile   → CI/CD pipeline
```

> "สำหรับ branch เราใช้ 3 ระดับครับ"
>
> - `main` — โค้ดที่พร้อม production แล้ว pipeline จะรันอัตโนมัติเมื่อ push
> - `dev` — รวมโค้ดก่อน merge ขึ้น main
> - `feature/*` — แต่ละคนพัฒนาแยก branch แล้วค่อย merge เข้า dev

---

### 1.2 App Code และ Dockerfile (5 คะแนน)

> "แอปประกอบด้วย 2 ส่วนครับ"
>
> **Backend** — .NET 8 ASP.NET Core Web API ใช้ Dapper เชื่อมต่อ MySQL  
> ทำงานที่ port 5154 มี Swagger UI และ `/metrics` สำหรับ Prometheus
>
> **Frontend** — React 18 + Vite ทำงานที่ port 5173 (dev) หรือ 80 (production)

*[แสดง Dockerfile ของ backend]*

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /src
COPY . .
RUN dotnet build ConstructionSaaS.Api.csproj -c Release

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=builder /src/bin/Release/net8.0/publish .
ENTRYPOINT ["dotnet", "ConstructionSaaS.Api.dll"]
```

> "Dockerfile ใช้ multi-stage build ครับ — stage แรก build โค้ด stage สองรันแอป ทำให้ image เล็กลงและปลอดภัยขึ้น"

---

### 1.3 README.md (2 คะแนน)

> "README อธิบายวิธีรันทั้งระบบครับ ทั้งแบบ Docker Compose สำหรับ dev และแบบ Kubernetes สำหรับ production มีทั้งคำสั่ง troubleshooting และ architecture diagram ด้วย"

> "ต่อไป พิชญ์สินีจะอธิบาย Jenkins pipeline ครับ"

---

# ผู้พูด 2: Jenkins CI/CD + Docker (2:30–5:00)
**Phase 2 — 25 คะแนน**

---

> "สวัสดีค่ะ หนูชื่อพิชญ์สินี จะมาอธิบาย Phase 2 เรื่อง Jenkins CI/CD และ Docker ค่ะ"

---

### 2.1 Jenkinsfile — ครบ 6 stages (10 คะแนน)

> "Jenkinsfile ของเรามีทั้งหมด 5 stage หลัก แต่ใน stage 'Test & Build' มี 3 stage ย่อยรันพร้อมกัน รวมแล้วครบ 6 ค่ะ"

```
1. Checkout
2. Test (parallel)
3. Docker Build Backend (parallel)
4. Docker Build Frontend (parallel)
5. Push to Hub
6. Deploy
```

*[แสดง Jenkinsfile หน้าจอ]*

> "ทำไมถึงใช้ parallel? เพราะ test กับ build ไม่ได้พึ่งพากัน รันพร้อมกันได้เลย ประหยัดเวลาไปประมาณ 2 นาทีต่อ build ค่ะ"

---

### 2.2 Webhook trigger อัตโนมัติ (5 คะแนน)

> "เวลาใครใน team push โค้ดขึ้น GitHub branch main — Jenkins จะรันอัตโนมัติทันทีค่ะ ไม่ต้องกดเอง"
>
> "ทำได้ด้วย GitHub Webhook ตั้งค่าที่:"
> - GitHub → Settings → Webhooks
> - Payload URL: `http://[jenkins-host]:8080/github-webhook/`
> - Trigger: Push event

*[แสดง Jenkins pipeline หลัง webhook trigger — เห็น build เริ่มอัตโนมัติ]*

---

### 2.3 Docker image build และ push ขึ้น Docker Hub (10 คะแนน)

> "หลัง test ผ่าน Jenkins สร้าง Docker image 2 ตัวค่ะ"

```bash
docker build -t mikedeni/mybrick-backend:${BUILD_NUMBER} ./app/backend
docker build -t mikedeni/mybrick-frontend:${BUILD_NUMBER} ./app/frontend
```

> "แต่ละ build จะได้ tag 2 ชื่อ — ชื่อแรกเป็นเลข build เช่น `:42` เพื่อ rollback ได้ ชื่อสองเป็น `:latest` สำหรับ deploy"
>
> "จากนั้น push ขึ้น Docker Hub พร้อมกัน 4 image ค่ะ"

```bash
docker push mikedeni/mybrick-backend:42 &
docker push mikedeni/mybrick-backend:latest &
docker push mikedeni/mybrick-frontend:42 &
docker push mikedeni/mybrick-frontend:latest &
wait
```

*[แสดง Docker Hub — เห็น repository mikedeni/mybrick-backend มี tags หลาย version]*

> "ตอนนี้ image พร้อมแล้ว ต่อไปโกวิทจะอธิบายว่า Terraform กับ Ansible เตรียม infrastructure ยังไงค่ะ"

---

# ผู้พูด 3: Terraform + Ansible + Kubernetes (5:00–7:30)
**Phase 3 (15 คะแนน) + Phase 4 (25 คะแนน)**

---

> "สวัสดีครับ ผมโกวิท จะอธิบาย 2 Phase ติดกันเลยครับ คือ Terraform+Ansible และ Kubernetes"

---

### 3.1 Terraform สร้าง infrastructure (7 คะแนน)

> "Terraform มีหน้าที่สร้าง Kubernetes namespace ชื่อ `production` ครับ"

```bash
cd terraform
terraform init
terraform plan    # ดูก่อนว่าจะสร้างอะไร
terraform apply   # สร้างจริง
```

*[แสดง terminal — เห็น `kubernetes_namespace.mybrick: Creation complete`]*

> "ทำไมต้องใช้ Terraform แทนที่จะ kubectl สร้าง namespace เอง? เพราะ Terraform เก็บ state ไว้ครับ รู้ว่า resource ไหนสร้างแล้ว ถ้า run ซ้ำก็ไม่สร้างซ้ำ และ rollback ได้"

---

### 3.2 Ansible configure environment (5 คะแนน)

> "Ansible ทำงานบน Jenkins host ครับ มีหน้าที่ตรวจสอบว่าเครื่องพร้อมก่อน deploy"

```bash
cd ansible
ansible-playbook -i inventory playbook.yml
```

> "สิ่งที่ Ansible ทำ:"
> 1. ตรวจว่า kubectl ติดตั้งอยู่ไหม
> 2. Set context ของ kubectl ให้ชี้ไปที่ OrbStack cluster
> 3. ตรวจว่า namespace `production` มีอยู่แล้วหรือยัง

---

### 3.3 ทั้งสองรวมอยู่ใน Jenkins Deploy stage (3 คะแนน)

> "ใน Jenkinsfile stage 'Deploy' จะรัน Terraform และ Ansible ก่อน แล้วค่อย deploy ครับ"

```groovy
stage('Deploy') {
    steps {
        sh 'kubectl apply -f k8s/'
        sh "kubectl set image deployment/mybrick-backend ..."
        sh "kubectl set image deployment/mybrick-frontend ..."
    }
}
```

> "Terraform กับ Ansible รันแค่ครั้งแรกครับ หลังจากนั้น Jenkins จัดการ deploy เองผ่าน kubectl"

---

### 3.4 Kubernetes Deployment — deployment.yaml ครบถ้วน (10 คะแนน)

> "ใน k8s/ folder มี manifest หลายไฟล์ครับ ที่สำคัญที่สุดคือ backend-deployment.yaml"

```yaml
spec:
  replicas: 2           # 2 pods เสมอ ถ้าตัวหนึ่งล่มยังมีอีกตัว
  selector:
    matchLabels:
      app: mybrick-backend
  template:
    spec:
      containers:
      - name: mybrick-backend
        image: mikedeni/mybrick-backend:latest
```

> "2 replicas หมายความว่าแอปยังทำงานได้ถ้า pod ตัวหนึ่งมีปัญหา"

---

### 3.5 services.yaml — NodePort (7 คะแนน)

> "Service เชื่อม pod เข้ากับโลกภายนอกครับ ใช้ NodePort"

```yaml
spec:
  type: NodePort
  ports:
  - port: 5154
    nodePort: 30154    # เข้าถึงจากนอก cluster ได้ที่ port นี้
```

| Service | NodePort | ใช้งาน |
|---------|----------|--------|
| mybrick-backend-svc | 30154 | API + /metrics |
| mybrick-frontend-svc | 30080 | หน้าเว็บ |

---

### 3.6 Pods running หลัง pipeline (8 คะแนน)

*[แสดง terminal จริง]*

```bash
kubectl get pods -n production
```

```
NAME                                READY   STATUS    RESTARTS   AGE
mybrick-backend-xxx-yyy             1/1     Running   0          2m
mybrick-backend-xxx-zzz             1/1     Running   0          2m
mybrick-frontend-xxx-aaa            1/1     Running   0          1m
mybrick-frontend-xxx-bbb            1/1     Running   0          1m
```

> "ทั้ง 4 pods Status เป็น Running ครับ เข้าแอปได้ที่ http://localhost:30080"

> "ต่อไปตะวันจะแสดง Prometheus และ Grafana ครับ"

---

# ผู้พูด 4: Prometheus + Grafana + Live Demo (7:30–10:00)
**Phase 5 (15 คะแนน) + Bonus (10 คะแนน)**

---

> "สวัสดีครับ ผมตะวัน Phase สุดท้ายเรื่อง monitoring และ live demo ครับ"

---

### 4.1 /metrics endpoint (5 คะแนน)

> "Backend มี endpoint `/metrics` ครับ Prometheus ใช้ดึงข้อมูล"

*[เปิด browser หรือ curl]*

```bash
curl http://localhost:30154/metrics
```

```
# HELP mybrick_projects_total Total number of projects
# TYPE mybrick_projects_total gauge
mybrick_projects_total 12

# HELP mybrick_expenses_total_baht Total spending in THB
# TYPE mybrick_expenses_total_baht gauge
mybrick_expenses_total_baht 485000

# HELP http_requests_received_total Total HTTP requests
http_requests_received_total{code="200"} 1547
```

> "metrics เหล่านี้มาจาก library prometheus-net ที่ register ไว้ใน Program.cs ครับ อัปเดตทุก 5 นาทีโดย MetricsBackgroundService"

---

### 4.2 Prometheus scrape สำเร็จ (5 คะแนน)

*[เปิด http://localhost:30090 หรือ http://localhost:9090]*

> "เข้า Prometheus แล้วไปที่ Status → Targets ครับ"
>
> "จะเห็น `mybrick-backend` แสดง **State: UP** หมายความว่า Prometheus ดึง metrics ได้สำเร็จทุก 15 วินาที"

*[แสดง Prometheus Targets page — เห็น UP สีเขียว]*

---

### 4.3 Grafana Dashboard ≥ 3 panels (5 คะแนน)

*[เปิด http://localhost:30300]*

> "Dashboard ของเรามี 6 panel ครับ เกินขั้นต่ำที่กำหนด"

| Panel | แสดงอะไร |
|-------|---------|
| Request Rate | จำนวน request ต่อวินาที |
| Error Rate | จำนวน error 5xx ต่อวินาที |
| Response Time p95 | ความเร็วตอบสนอง |
| Pod Health | backend up หรือ down |
| Total Projects | จำนวนโปรเจคในระบบ |
| Total Expenses (THB) | ยอดค่าใช้จ่ายรวม |

*[เลื่อนดู dashboard จริง — เห็นกราฟและตัวเลข]*

---

### 4.4 Live Demo (Bonus — 5 คะแนน)

> "ตอนนี้จะ demo สด ครับ — push code แล้วดู pipeline รันอัตโนมัติจนถึง pods running"

**ขั้นตอน:**

```bash
# แก้ไขโค้ดอะไรก็ได้ เช่น version string
git add .
git commit -m "demo: trigger pipeline"
git push origin main
```

*[เปิด Jenkins — เห็น build เริ่มอัตโนมัติภายใน 10 วินาที]*

> "เห็นไหมครับ push ปุ๊บ Jenkins รันเลย ไม่ต้องกดอะไรเพิ่ม"

*[รอ pipeline จบ — แสดง green stages ทั้งหมด]*

*[รัน kubectl get pods — เห็น pods ใหม่ขึ้นมา]*

> "pipeline ใช้เวลาประมาณ 4-5 นาที ครับ จาก git push จนถึง pods running ใน production"

---

### 4.5 Architecture Diagram (Bonus — 3 คะแนน)

*[ชี้ไปที่ diagram ใน README หรือ slide]*

```
Developer
    │ git push
    ▼
GitHub ──webhook──▶ Jenkins
                       │
          ┌────────────┼────────────┐
          ▼            ▼            ▼
        Test     Build Backend  Build Frontend
          └────────────┼────────────┘
                       ▼
                  Docker Hub
                       │
              ┌────────┴────────┐
              ▼                 ▼
          Terraform           Ansible
        (namespace)     (kubectl config)
              └────────┬────────┘
                       ▼
               Kubernetes (production)
              ┌────────────────────┐
              │  backend × 2 pods  │
              │  frontend × 2 pods │
              └────────────────────┘
                       │
              ┌────────┴────────┐
              ▼                 ▼
          Prometheus ──────▶ Grafana
```

> "ทุก component เชื่อมกันหมดครับ ตั้งแต่ developer กด push จนถึง dashboard monitoring แสดงผล"

---

### สรุป + Q&A (Bonus — 2 คะแนน)

> "สรุปสั้นๆ ครับ — MyBrick เป็นระบบ construction management ที่ deploy แบบ automated ทั้งหมด:"
>
> - **Phase 1** Git + branching + Dockerfile ✅
> - **Phase 2** Jenkins 6 stages + webhook + Docker Hub ✅
> - **Phase 3** Terraform สร้าง namespace + Ansible config kubectl ✅
> - **Phase 4** Kubernetes 2 replicas + NodePort + pods running ✅
> - **Phase 5** /metrics + Prometheus scrape + Grafana 6 panels ✅
>
> "พร้อมรับคำถามครับ"

---

# Checklist ก่อนนำเสนอ

- [ ] Jenkins job "mybrick" พร้อมและเชื่อม GitHub webhook แล้ว
- [ ] Pods ทุกตัวใน namespace `production` Status: Running
- [ ] เปิด Prometheus Targets — mybrick-backend แสดง **UP**
- [ ] เปิด Grafana dashboard — ทุก panel แสดงข้อมูล
- [ ] Docker Hub มี image tags ล่าสุด
- [ ] เตรียม terminal สำหรับ live demo (kubectl watch พร้อม)
- [ ] แต่ละคนซักซ้อมสคริปต์ของตัวเองแล้ว

---

# คำถามที่อาจถูกถาม (Q&A Prep)

**"ทำไมต้องใช้ Terraform ถ้าแค่สร้าง namespace?"**
> Terraform เก็บ state ครับ รู้ว่าสร้างอะไรไปแล้ว ถ้า infra ใหญ่ขึ้น (เพิ่ม RDS, VPC, etc.) ใช้ได้เลยโดยไม่ต้องเปลี่ยนโครงสร้าง

**"ทำไม Ansible ไม่อยู่ใน pipeline?"**
> Ansible config เครื่อง host ครับ ทำแค่ครั้งเดียวตอนตั้งค่า Jenkins server ไม่ต้องรันทุก build

**"Rolling update คืออะไร?"**
> Kubernetes ไม่ kill pod เก่าทั้งหมดพร้อมกันครับ — เลิกทีละ pod แล้วเอาใหม่ขึ้นทดแทน แอปไม่ดับระหว่าง deploy

**"ถ้า test fail จะเกิดอะไร?"**
> Pipeline หยุดทันทีครับ ไม่มีการ build หรือ push image ออกไป pod เก่ายังรันต่อไปปกติ

**"2 replicas ช่วยอะไร?"**
> ถ้า pod หนึ่งล่ม Kubernetes route traffic ไปที่อีก pod โดยอัตโนมัติครับ ผู้ใช้ไม่รู้สึกว่ามีปัญหา
