---
title: MyBrick Jenkins Pipeline Presentation — คู่มือการนำเสนอ 10 นาที
subtitle: สคริปต์การนำเสนอ 4 คน (2.5 นาทีต่อคน) | GitHub Webhook → Deploy
---

# 🎬 MyBrick Jenkins Pipeline Presentation — 10 นาที

**ผู้ฟังเป้าหมาย:** อาจารย์ที่ประเมินผลงาน  
**รูปแบบ:** 4 คนพูด × 2.5 นาทีต่อคน  
**เป้าหมาย:** อธิบาย Jenkins CI/CD pipeline จาก GitHub webhook จนถึง deployment ใน Kubernetes

---

## 📋 Jenkins Pipeline Flow

```
GitHub Webhook
    ↓
STAGE 1: Checkout (ผู้พูด 1)
    ↓
STAGE 2: Test & Build (parallel) (ผู้พูด 2)
    ├─ dotnet test
    ├─ docker build backend
    └─ docker build frontend
    ↓
STAGE 3: Push to Hub (parallel) (ผู้พูด 3)
    ├─ push backend:tag & backend:latest
    └─ push frontend:tag & frontend:latest
    ↓
STAGE 4: Deploy to Kubernetes (ผู้พูด 4)
    ├─ kubectl apply -f k8s/
    ├─ kubectl set image deployment/mybrick-backend
    └─ kubectl set image deployment/mybrick-frontend
    ↓
✅ Production Running
```

---

# ผู้พูด 1: GitHub Webhook + Checkout (0:00–2:30)

## สคริปต์ (อ่านในระดับปกติ)

**บทเปิด (15s)**
> "สวัสดีครับ ผมชื่อ [ชื่อ] วันนี้ผมจะอธิบายวิธีการที่ Jenkins ทำให้ workflow ของเราเป็นอัตโนมัติ โดยเริ่มต้นจากเมื่อเรา push code ไปที่ GitHub"

**GitHub Webhook Trigger (40s)**
> "ขั้นแรก: GitHub webhook ทำการเชื่อมต่อเกาะ Jenkins ทุกครั้งที่มีการ push ไปยัง main branch"
> 
> "ในการตั้งค่า GitHub Webhook:"
> - Payload URL: `http://localhost:8080/github-webhook/`
> - Content type: `application/json`
> - Trigger: `Just the push event`
> - Status: ✅ Active
>
> "เมื่อเหตุการณ์ push เกิดขึ้น GitHub ส่ง payload ไปยัง Jenkins ทันที"

**Jenkinsfile + Pipeline Job (30s)**
> "Jenkinsfile ในรูท repository นี้ประกอบด้วยทั้ง pipeline logic นี้ Jenkins อ่านไฟล์นี้และดำเนินการตามคำสั่งเพื่อสร้าง, ทดสอบ และปรับใช้"
> *[แสดงภาพหน้าจอ Jenkinsfile]*

**Stage 1: Checkout (35s)**
> "ขั้นแรกของ pipeline: Checkout Stage"
> ```groovy
> stage('Checkout') {
>     steps {
>         checkout scm
>     }
> }
> ```
> "SCM = Source Control Management ซึ่งเป็น Git repository นี้ Jenkins ดึง code ล่าสุดจาก main branch"
> *[แสดง terminal output: "Cloning repository..." ← "Cloning completed"]*

**Environment Variables (20s)**
> "ก่อนเริ่มต้น pipeline ได้กำหนด environment variables:"
> ```groovy
> BACKEND_IMAGE  = "mikedeni/mybrick-backend"
> FRONTEND_IMAGE = "mikedeni/mybrick-frontend"
> IMAGE_TAG      = "${BUILD_NUMBER}"
> ```
> "IMAGE_TAG ใช้ BUILD_NUMBER (ตัวเลขเพิ่มขึ้นต่อการสร้าง) เพื่อติดตามและ version image แต่ละตัว"

**Closing (10s)**
> "โค้ด pull เสร็จแล้ว ต่อไป [ผู้พูด 2] จะแสดงให้เห็นว่าขั้นตอน Test & Build ทำงานแบบ parallel อย่างไร"

---

## Visual Assets สำหรับผู้พูด 1

### ภาพหน้าจอ 1: GitHub Webhook Configuration
```
Webhooks / Manage Webhook

Payload URL:           http://localhost:8080/github-webhook/
Content type:          application/json
Triggers:              ✓ Just the push event
                       ✓ Pushes to refs/heads/main
Status:                ✅ Recent Deliveries (green)
Last delivery:         2 minutes ago — Delivery success ✅
```

### ภาพหน้าจอ 2: Jenkins Pipeline Job Config
```
Pipeline job: mybrick
Definition: Pipeline script from SCM
SCM: Git
Repository URL: https://github.com/mikedeni/serverless-project.git
Branch: */main
Script path: Jenkinsfile
Build Trigger: ✓ GitHub hook trigger for GITScm polling
```

### ภาพหน้าจอ 3: Checkout Stage Output
```
[Pipeline] Start of Pipeline
[Pipeline] node
Running on Jenkins in /var/jenkins_home/workspace/mybrick
[Pipeline] {
[Pipeline] stage
[Pipeline] { (Checkout)
[Pipeline] checkout
Cloning the remote Git repository
Cloning repository https://github.com/mikedeni/serverless-project.git
 > git init /var/jenkins_home/workspace/mybrick
 > git fetch --tags --progress https://github.com/mikedeni/serverless-project.git +refs/heads/main:refs/remotes/origin/main
 > git checkout -b main refs/remotes/origin/main
Commit message: "feat: add new endpoint"
[Pipeline] }
[Pipeline] // stage
```

### ภาพหน้าจอ 4: Jenkinsfile Environment Block
```groovy
environment {
    PATH = "/opt/homebrew/bin:/usr/local/bin:${PATH}"
    BACKEND_IMAGE  = "mikedeni/mybrick-backend"
    FRONTEND_IMAGE = "mikedeni/mybrick-frontend"
    IMAGE_TAG      = "${BUILD_NUMBER}"
    DOCKER_BUILDKIT = "1"
}
```

---

# ผู้พูด 2: Test & Build (Parallel) (2:30–5:00)

## สคริปต์ (อ่านในจังหวะสม่ำเสมอ)

**บทเปิด (10s)**
> "สวัสดีครับ ผมชื่อ [ชื่อ] ตอนนี้ code ของเราได้ถูก pull แล้ว ขั้นตอนถัดไป: Test & Build ที่ทำงาน parallel"

**Parallel Execution (30s)**
> "Stage นี้ทำสามสิ่ง พร้อมกัน ไม่เป็นลำดับ:"
> ```groovy
> stage('Test & Build (parallel)') {
>     parallel {
>         stage('Test') { ... }
>         stage('Docker Build Backend') { ... }
>         stage('Docker Build Frontend') { ... }
>     }
> }
> ```
> "ทำไม parallel? เพราะการสร้าง Docker image backend ไม่ได้ขึ้นอยู่กับ frontend และการทดสอบเป็นอิสระ ดังนั้นการทำทั้ง 3 อย่างพร้อมกันจะเร็วขึ้น 3 เท่า"

**Test Stage (40s)**
> "ขั้นที่ 1: ทดสอบ"
> ```bash
> dotnet test app/backend.tests/ConstructionSaaS.Tests.csproj -v minimal
> ```
> *[แสดง output: Test Run Successful. Total tests: 25. Passed: 25]*
> "xUnit tests ทั้งหมด 25 ตัวต้องผ่าน ถ้า fail เพียง 1 ตัว pipeline จะหยุด ไม่มี image ถูกสร้าง"

**Docker Build Backend (25s)**
> "ขั้นที่ 2a: สร้าง Backend Image"
> ```bash
> docker build -t mikedeni/mybrick-backend:${BUILD_NUMBER} \
>             -t mikedeni/mybrick-backend:latest \
>             ./app/backend
> ```
> "สร้าง 2 tags: one with BUILD_NUMBER (v42), one with latest"

**Docker Build Frontend (25s)**
> "ขั้นที่ 2b: สร้าง Frontend Image"
> ```bash
> docker build -t mikedeni/mybrick-frontend:${BUILD_NUMBER} \
>             -t mikedeni/mybrick-frontend:latest \
>             ./app/frontend
> ```
> "เหมือนกัน 2 tags สำหรับ frontend"

**Completion (10s)**
> "เมื่อ test ผ่านและ 2 images สร้างเสร็จ stage นี้จบสิ้น ต่อไป [ผู้พูด 3] จะ push images เหล่านี้ไปยัง Docker Hub"

---

## Visual Assets สำหรับผู้พูด 2

### ภาพหน้าจอ 1: Parallel Execution Timeline
```
Timeline (Parallel):

Test                          [████████] 45s
Docker Build Backend         [████████████████] 90s
Docker Build Frontend        [████████████████] 90s
                                              ↓ All complete at 90s total

Sequential (old way):
Test                          [████████] 45s
Docker Build Backend                        [████████████████] 90s
Docker Build Frontend                                          [████████████████] 90s
                                                                                        Total 245s

Parallel saves: 245s - 90s = 155s (63% faster)
```

### ภาพหน้าจอ 2: Test Output
```
[Pipeline] stage
[Pipeline] { (Test & Build (parallel))
[Pipeline] parallel
[Pipeline] { (Branch: Test)
[Pipeline] { (Branch: Docker Build Backend)
[Pipeline] { (Branch: Docker Build Frontend)
[Pipeline] sh
  Build started at 2025-05-15 14:22:30
  
Test Run Successful.
Total tests: 25
  Passed: 25
  Failed: 0
Duration: 3.245 seconds

✅ Test Stage PASSED
```

### ภาพหน้าจอ 3: Docker Build Output
```
[Pipeline] sh (Docker Build Backend)
Building image: mikedeni/mybrick-backend:42
[+] Building 45.2s (15/15) FINISHED
 => [internal] load build definition from Dockerfile
 => [builder 1/4] FROM mcr.microsoft.com/dotnet/sdk:10.0
 => [builder 2/4] WORKDIR /src
 => [builder 3/4] COPY . .
 => [builder 4/4] RUN dotnet build ConstructionSaaS.csproj
 => [stage-2 1/2] FROM mcr.microsoft.com/dotnet/aspnet:10.0
 => [stage-2 2/2] COPY --from=builder ...
 => => naming to docker.io/mikedeni/mybrick-backend:42
 => => naming to docker.io/mikedeni/mybrick-backend:latest

✅ Backend image built successfully
```

### ภาพหน้าจอ 4: All 3 Stages Completed
```
[Pipeline] }
[Pipeline] // parallel

✅ Stage 'Test & Build (parallel)' completed in 92 seconds
  - Test: PASSED
  - Docker Build Backend: PASSED
  - Docker Build Frontend: PASSED
  
Ready for Push stage
```

---

# ผู้พูด 3: Push to Hub (Parallel) (5:00–7:30)

## สคริปต์ (อ่านด้วยความกระตือรือร้น)

**บทเปิด (10s)**
> "สวัสดีครับ ผมชื่อ [ชื่อ] ตอนนี้เรามี 2 Docker images พร้อมแล้ว ขั้นตอนนี้ push พวกมันไปยัง Docker Hub"

**Docker Hub Credentials (30s)**
> "ก่อน push ต้องเข้าสู่ระบบ Docker ด้วย credentials"
> ```groovy
> withCredentials([usernamePassword(
>     credentialsId: 'dockerhub-credentials',
>     usernameVariable: 'DOCKER_USER',
>     passwordVariable: 'DOCKER_PASS'
> )]) {
>     echo $DOCKER_PASS | docker login -u $DOCKER_USER --password-stdin
> }
> ```
> "Jenkins เข้าถึง credentials ที่จัดเก็บไว้อย่างปลอดภัยจาก credential store"
> *[แสดง Jenkins Credentials UI]*

**Parallel Push (50s)**
> "Push 4 images parallel:"
> ```bash
> docker push mikedeni/mybrick-backend:${BUILD_NUMBER} &
> docker push mikedeni/mybrick-backend:latest &
> docker push mikedeni/mybrick-frontend:${BUILD_NUMBER} &
> docker push mikedeni/mybrick-frontend:latest &
> wait
> ```
> "4 push พร้อมกัน (& เรียกใช้ background) ด้วยคำสั่ง `wait` Jenkins รอให้ทั้ง 4 เสร็จ"
> 
> *[แสดง output: 4 push progress bars]*
> ```
> [Pipeline] sh
> $ docker push mikedeni/mybrick-backend:42
> The push refers to repository [docker.io/mikedeni/mybrick-backend]
> 42: digest: sha256:abc123... size: 2048
> ✅ Pushed successfully
> 
> $ docker push mikedeni/mybrick-backend:latest
> latest: digest: sha256:abc123... size: 2048
> ✅ Pushed successfully
> 
> $ docker push mikedeni/mybrick-frontend:42
> 42: digest: sha256:def456... size: 1024
> ✅ Pushed successfully
> 
> $ docker push mikedeni/mybrick-frontend:latest
> latest: digest: sha256:def456... size: 1024
> ✅ Pushed successfully
> ```
> "ทั้ง 4 push เสร็จใน ~60 วินาที ถ้าทำลำดับจะใช้ 240 วินาที"

**Docker Hub Verification (20s)**
> "เมื่อเสร็จ images ยังคงอยู่บน Docker Hub:"
> *[แสดง Docker Hub repository]*
> - `mikedeni/mybrick-backend:42` ✅
> - `mikedeni/mybrick-backend:latest` ✅
> - `mikedeni/mybrick-frontend:42` ✅
> - `mikedeni/mybrick-frontend:latest` ✅

**Closing (10s)**
> "ตอนนี้ images อยู่บน Docker Hub และพร้อมสำหรับ Kubernetes ที่จะดึง ต่อไป [ผู้พูด 4] จะแสดง Deploy stage"

---

## Visual Assets สำหรับผู้พูด 3

### ภาพหน้าจอ 1: Jenkins Credentials Storage
```
Manage Jenkins → Credentials → Global

ID: dockerhub-credentials
Username: mikedeni
Password: ••••••••••••••
```

### ภาพหน้าจอ 2: Docker Login
```
[Pipeline] sh
$ docker login -u mikedeni --password-stdin
WARNING! Your password will be stored unencrypted in /var/jenkins_home/.docker/config.json.

✅ Login Succeeded
```

### ภาพหน้าจอ 3: Parallel Push Output
```
[Pipeline] sh (Push to Hub)
Running parallel pushes:

 backend:42       [████████████████░░░░░░░] 45%  (850MB)
 backend:latest   [████████░░░░░░░░░░░░░░░░░] 30%  (850MB)
 frontend:42      [████████████████░░░░░░░░░] 50%  (420MB)
 frontend:latest  [████████░░░░░░░░░░░░░░░░░] 28%  (420MB)

 backend:42         ✅ Pushed (digest: sha256:abc...)
 backend:latest     ✅ Pushed (digest: sha256:abc...)
 frontend:42        ✅ Pushed (digest: sha256:def...)
 frontend:latest    ✅ Pushed (digest: sha256:def...)

Total time: 62 seconds
```

### ภาพหน้าจอ 4: Docker Hub Registry
```
Repository: mikedeni/mybrick-backend
Tags:
  - 42          (pushed 2 minutes ago)
  - 41          (pushed 5 minutes ago)
  - 40          (pushed 10 minutes ago)
  - latest      (points to 42)

Repository: mikedeni/mybrick-frontend
Tags:
  - 42          (pushed 2 minutes ago)
  - 41          (pushed 5 minutes ago)
  - 40          (pushed 10 minutes ago)
  - latest      (points to 42)
```

---

# ผู้พูด 4: Deploy to Kubernetes (7:30–10:00)

## สคริปต์ (อ่านด้วยจังหวะ)

**บทเปิด (15s)**
> "สวัสดีครับ ผมชื่อ [ชื่อ] ตอนนี้ images อยู่บน Docker Hub แล้ว ขั้นตอนสุดท้าย: Deploy เข้าไปใน Kubernetes production cluster"

**Pre-Deploy Infrastructure (20s)**
> "Kubernetes cluster (production namespace) ต้องเตรียมพร้อมแล้ว (จาก Terraform + Ansible)"
> ```bash
> kubectl get namespace production
> kubectl get pods -n production
> ```
> "Deployment objects พร้อม (backend, frontend, MySQL, Prometheus, Grafana)"

**Stage 4a: Apply Manifests (30s)**
> "ขั้นที่ 1: Apply YAML manifests"
> ```bash
> kubectl apply -f k8s/
> ```
> *[แสดง output]*
> ```
> namespace/production unchanged
> deployment.apps/mybrick-backend unchanged
> deployment.apps/mybrick-frontend unchanged
> service/mybrick-backend-svc unchanged
> service/mybrick-frontend-svc unchanged
> mysql-statefulset.apps/mysql unchanged
> configmap/prometheus-config unchanged
> deployment.apps/prometheus unchanged
> deployment.apps/grafana unchanged
> ```
> "kubectl apply อ่าน manifests ทั้งหมด ถ้ามีการเปลี่ยนแปลง deployment จะอัพเดต"

**Stage 4b: Set New Image — Backend (30s)**
> "ขั้นที่ 2a: Update backend deployment ให้ใช้ image ใหม่"
> ```bash
> kubectl set image deployment/mybrick-backend \
>     mybrick-backend=mikedeni/mybrick-backend:42 \
>     -n production
> ```
> *[แสดง output]*
> ```
> deployment.apps/mybrick-backend image updated
> ```
> "Kubernetes ทำ rolling update: เลิกใช้ pod เก่า นำขึ้น pod ใหม่ด้วย image 42 ไม่มี downtime"

**Stage 4c: Set New Image — Frontend (30s)**
> "ขั้นที่ 2b: Update frontend deployment ให้ใช้ image ใหม่"
> ```bash
> kubectl set image deployment/mybrick-frontend \
>     mybrick-frontend=mikedeni/mybrick-frontend:42 \
>     -n production
> ```
> *[แสดง output]*
> ```
> deployment.apps/mybrick-frontend image updated
> ```
> "ทำเช่นเดียวกัน rolling update frontend"

**Post-Deployment Verification (20s)**
> "Pipeline สิ้นสุด ตรวจสอบให้แน่ใจว่า pods ทั้งหมดรับการอัปเดต:"
> ```bash
> kubectl get pods -n production --watch
> ```
> *[แสดง all pods rolling to Running]*
> ```
> NAME                                READY   STATUS    AGE
> mybrick-backend-5f7d4c8a9-xyz11     1/1     Running   2m  (new, image:42)
> mybrick-backend-5f7d4c8a9-abc22     1/1     Running   2m  (new, image:42)
> mybrick-frontend-9k2l3m4n5-def33    1/1     Running   1m  (new, image:42)
> mybrick-frontend-9k2l3m4n5-ghi44    1/1     Running   1m  (new, image:42)
> ```

**Live Service Verification (10s)**
> "Frontend accessible ที่ http://localhost:30080 ✅"
> "Backend Swagger ที่ http://localhost:30154/swagger ✅"
> "Metrics เก็บข้อมูล Prometheus ✅"
> "Grafana dashboard แสดง live metrics ✅"

**Closing (5s)**
> "จบ pipeline ทั้งหมด: GitHub webhook → checkout → test & build (parallel) → push (parallel) → deploy → running ใน Kubernetes ผ่านลักษณะที่เป็นอัตโนมัติทั้งหมด"

---

## Visual Assets สำหรับผู้พูด 4

### ภาพหน้าจอ 1: kubectl apply -f k8s/
```
[Pipeline] stage
[Pipeline] { (Deploy)
[Pipeline] sh (kubectl apply)
namespace/production unchanged
deployment.apps/mybrick-backend unchanged
deployment.apps/mybrick-frontend unchanged
service/mybrick-backend-svc unchanged
service/mybrick-frontend-svc unchanged
statefulset.apps/mysql unchanged
configmap/prometheus-config unchanged
deployment.apps/prometheus unchanged
deployment.apps/grafana unchanged

All manifests applied ✅
```

### ภาพหน้าจอ 2: Set Image Backend
```
[Pipeline] sh (kubectl set image backend)
$ kubectl set image deployment/mybrick-backend \
  mybrick-backend=mikedeni/mybrick-backend:42 \
  -n production

deployment.apps/mybrick-backend image updated

Rolling Update Started:
Old pods (image:41):     2 → terminating
New pods (image:42):     0 → 1 → 2 (running)
```

### ภาพหน้าจอ 3: Set Image Frontend
```
[Pipeline] sh (kubectl set image frontend)
$ kubectl set image deployment/mybrick-frontend \
  mybrick-frontend=mikedeni/mybrick-frontend:42 \
  -n production

deployment.apps/mybrick-frontend image updated

Rolling Update Started:
Old pods (image:41):     2 → terminating
New pods (image:42):     0 → 1 → 2 (running)
```

### ภาพหน้าจอ 4: All Pods Running (Post-Deploy)
```
$ kubectl get pods -n production

NAME                                READY   STATUS    RESTARTS   AGE
mybrick-backend-5f7d4c8a9-xyz11     1/1     Running   0          2m
mybrick-backend-5f7d4c8a9-abc22     1/1     Running   0          2m
mybrick-frontend-9k2l3m4n5-def33    1/1     Running   0          1m
mybrick-frontend-9k2l3m4n5-ghi44    1/1     Running   0          1m
mysql-0                             1/1     Running   0          7d
prometheus-5b8c3e2f9-xyz99          1/1     Running   0          7d
grafana-6d7f4e3a1-abc88             1/1     Running   0          7d

✅ All 7 pods healthy
```

### ภาพหน้าจอ 5: Complete Jenkins Pipeline
```
[Pipeline] }
[Pipeline] // node
[Pipeline] End of Pipeline

✅ Jenkins Pipeline Complete

Pipeline Duration: 4m 32s
  Stage 'Checkout'                                 30s
  Stage 'Test & Build (parallel)'                  92s
  Stage 'Push to Hub (parallel)'                   62s
  Stage 'Deploy'                                   38s
  ──────────────────────────────────────
  Total                                         4m 32s
```

---

# 📊 Presentation Checklist

**ก่อนการนำเสนอ:**
- [ ] ผู้พูดทั้ง 4 คนมีสคริปต์ของพวกเขา
- [ ] แต่ละคนมีภาพหน้าจออพร้อม (โทรศัพท์/แล็ปท็อป)
- [ ] ตั้งค่า Timer สำหรับ 10 นาทีทั้งหมด (2:30 ต่อคน)
- [ ] GitHub repository เชื่อมต่อกับ Jenkins webhook ✅
- [ ] Jenkins job "mybrick" created ✅
- [ ] Kubernetes production namespace พร้อม ✅
- [ ] Docker Hub account และ credentials พร้อม ✅
- [ ] อาจเตรียม recent successful pipeline run ให้พร้อม (สำหรับ replay หรือการแสดงสด)

**ระหว่างการนำเสนอ:**
- พูดอย่างชัดเจน ไม่รีบร้อน
- ผู้พูด 1: แสดง GitHub webhook config + Jenkinsfile
- ผู้พูด 2: แสดง Jenkins build log ของ parallel test & build
- ผู้พูด 3: แสดง Docker Hub registry + push output
- ผู้พูด 4: แสดง kubectl get pods terminal + Grafana dashboard
- (Optional) Live demo: push change → Jenkins auto-trigger → full pipeline run
- ตั้งคำถามไว้จนกว่าผู้พูดทั้ง 4 คนเสร็จสิ้น

**หลังการนำเสนอ:**
- พร้อมอธิบายการออกแบบ (ทำไม parallel? ทำไม rolling update?)
- พร้อมแสดง logs ของ pipeline อ้างอิง
- มีคำสั่ง troubleshooting พร้อม

---

# 🔗 ข้อมูลอ้างอิง

**Jenkinsfile Location:** `/Users/mikedeni/Projects/serverless-project/Jenkinsfile`

**Key Files:**
- `Jenkinsfile` — pipeline definition
- `k8s/` — Kubernetes manifests
- `app/backend/Dockerfile` — backend image
- `app/frontend/Dockerfile` — frontend image

**Useful Commands:**
```bash
# View latest Jenkins build
jenkins logs | tail -f

# View current pod status
kubectl get pods -n production --watch

# View image tags on Docker Hub
docker search mikedeni/mybrick-backend
docker pull mikedeni/mybrick-backend:latest

# View Grafana dashboard
open http://localhost:30300
```
