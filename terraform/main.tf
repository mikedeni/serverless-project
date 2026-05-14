terraform {
  required_providers {
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.0"
    }
  }
}

provider "kubernetes" {
  config_path    = "~/.kube/config"
  config_context = "orbstack"
  insecure       = true
}

resource "kubernetes_namespace" "mybrick" {
  metadata {
    name = var.namespace
    labels = {
      app = var.app_name
    }
  }
}
