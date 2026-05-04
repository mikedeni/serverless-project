output "namespace" {
  description = "Kubernetes namespace created for MyBrick"
  value       = kubernetes_namespace.mybrick.metadata[0].name
}
