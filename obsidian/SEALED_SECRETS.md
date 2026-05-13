---
title: Sealed Secrets — Credential Encryption
tags:
  - security
  - kubernetes
  - secrets
  - encryption
course: ENG23 3074
---

# Sealed Secrets — Credential Encryption

> Encrypt Kubernetes secrets at rest using RSA 4096 + AES-256-GCM. Safe to commit to git.

---

## Why Sealed Secrets?

| Problem | Solution |
|---------|----------|
| Plaintext credentials in YAML | Encrypted via RSA 4096 + AES-256-GCM |
| Secrets readable in git history | Only decryptable in cluster with private key |
| Manual secret management | Automated encryption/decryption |
| No audit trail | Sealed Secrets logs all operations |

---

## How It Works

### Encryption Algorithm

**2-layer encryption:**

1. **AES-256-GCM** — symmetric cipher
   - Encrypts your secret (password, key, etc.)
   - 256-bit key (very strong)
   - Fast encryption for large data

2. **RSA 4096** — asymmetric cipher
   - Encrypts the AES-256 key
   - Public key: encrypt
   - Private key: decrypt (cluster only)
   - Slow but secure for key wrapping

3. **Base64** — encoding (not encryption)
   - Sealed secret stored as base64 in YAML
   - Readable in files, but encrypted layer prevents decryption

### Flow Diagram

```
Create Secret (plaintext)
  ↓
AES-256-GCM encrypts data
  ↓
RSA 4096 public key wraps AES key
  ↓
Base64 encode both → SealedSecret YAML
  ↓
Commit to git (safe, encrypted)
  ↓
Apply to K8s cluster
  ↓
SealedSecrets controller detects SealedSecret
  ↓
RSA 4096 private key (in cluster) unwraps AES key
  ↓
AES-256-GCM decrypts data
  ↓
Create plain Secret in memory
  ↓
Pod mounts Secret (plaintext in memory only)
```

---

## Installation

```bash
# Install Sealed Secrets controller (kube-system namespace)
kubectl apply -f https://github.com/bitnami-labs/sealed-secrets/releases/download/v0.24.0/controller.yaml -n kube-system

# Install kubeseal CLI
brew install kubeseal
```

---

## Create Sealed Secret (Manual)

### Method 1: One-liner

```bash
kubectl create secret generic my-secret \
  --from-literal=password=MyPassword123 \
  -n production --dry-run=client -o yaml | \
  kubeseal > sealed-my-secret.yaml

kubectl apply -f sealed-my-secret.yaml
```

### Method 2: Step-by-step

```bash
# 1. Create plain secret YAML
kubectl create secret generic my-secret \
  --from-literal=key=value \
  -n production --dry-run=client -o yaml > my-secret.yaml

# 2. Encrypt with kubeseal
kubeseal < my-secret.yaml > sealed-my-secret.yaml

# 3. Delete plain secret
rm my-secret.yaml

# 4. Apply sealed secret
kubectl apply -f sealed-my-secret.yaml
```

---

## Project Implementation

### Sealed Secrets Created

| File | Contents | Algorithm |
|------|----------|-----------|
| `k8s/sealed-app-secrets.yaml` | ConnectionString, Jwt__Key | RSA 4096 + AES-256-GCM |
| `k8s/sealed-mysql-credentials.yaml` | MySQL user/password | RSA 4096 + AES-256-GCM |

### How Backend Uses Secrets

```yaml
# k8s/backend-deployment.yaml
env:
  - name: ConnectionStrings__DefaultConnection
    valueFrom:
      secretKeyRef:
        name: app-secrets
        key: ConnectionString
  - name: Jwt__Key
    valueFrom:
      secretKeyRef:
        name: app-secrets
        key: Jwt__Key
```

### How MySQL Uses Secrets

```yaml
# k8s/mysql.yaml (deployment)
env:
  - name: MYSQL_PASSWORD
    valueFrom:
      secretKeyRef:
        name: mysql-credentials
        key: MYSQL_PASSWORD
```

---

## Security Guarantees

✅ **Encrypted at rest** — AES-256-GCM
✅ **Encrypted in transit** — HTTPS to K8s API
✅ **Private key stays in cluster** — can't decrypt outside
✅ **Safe in git** — encrypted YAML is unreadable
✅ **No plaintext in etcd** — only sealed form stored
✅ **Automatic decryption** — no manual key management

❌ **Not encrypted in memory** — pod reads plaintext
❌ **Not encrypted on disk in pod** — mounted as plaintext volumes

---

## Common Commands

```bash
# List sealed secrets
kubectl get sealedsecrets -n production

# Verify secret auto-decrypted
kubectl get secrets -n production

# Check what's in sealed secret (encrypted)
kubectl get sealedsecrets -n production sealed-app-secrets -o yaml

# Check decrypted secret (plaintext in cluster only)
kubectl get secret app-secrets -n production -o yaml

# Create new sealed secret
echo -n "MySecret" | kubectl create secret generic my-secret \
  --from-file=/dev/stdin -n production --dry-run=client -o yaml | \
  kubeseal > sealed-my-secret.yaml

# Re-seal with new key (if cluster key rotates)
kubeseal --recovery-unseal < sealed-my-secret.yaml | \
kubeseal > sealed-my-secret-new.yaml
```

---

## Gitignore Rules

```
# Block raw secrets (plaintext)
k8s/*-secret.yaml
k8s/*-credentials.yaml

# Allow sealed secrets (encrypted)
!k8s/sealed-*.yaml
```

Pattern prevents accidental commit of unencrypted secrets.

---

## Troubleshooting

**Issue: "Cannot find seal key" error**
- Sealed Secrets controller not running in kube-system
- Check: `kubectl get pods -n kube-system | grep sealed`

**Issue: Secret not decrypted in pod**
- Check secret mounted in pod: `kubectl describe pod <name> -n production`
- Check secret exists: `kubectl get secrets -n production`
- Check pod logs: `kubectl logs <pod> -n production`

**Issue: Can't create sealed secret**
- kubeseal CLI not installed: `brew install kubeseal`
- K8s context wrong: `kubectl config current-context`
- Namespace mismatch: use `-n production` in both commands

---

## References

- [[INSTALL_GUIDE]] — Step-by-step setup
- [[PROJECT_GUIDE]] — Phase completion
- [Sealed Secrets GitHub](https://github.com/bitnami-labs/sealed-secrets)
- [K8s Secrets Best Practices](https://kubernetes.io/docs/concepts/configuration/secret/)

---

## 🔗 Related

- [[HOME]] — Vault index
- [[PIPELINE_CHECKLIST]] — Deliverables
- [[COMMANDS_CHEATSHEET]] — Quick reference
