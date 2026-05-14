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

data "kubernetes_namespace" "mybrick" {
  metadata {
    name = var.namespace
  }
}
