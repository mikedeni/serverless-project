output "namespace" {
  description = "Kubernetes namespace for MyBrick"
  value       = data.kubernetes_namespace.mybrick.metadata[0].name
}
