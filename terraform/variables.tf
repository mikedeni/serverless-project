variable "namespace" {
  description = "Kubernetes namespace for MyBrick"
  type        = string
  default     = "production"
}

variable "app_name" {
  description = "Application name label"
  type        = string
  default     = "mybrick"
}
