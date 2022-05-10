terraform {
  required_providers {
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = ">= 2.0.0"
    }
  }

  backend "s3" {
    encrypt = true

    #dynamodb_table = "terraform-lock"
  }

}

provider "kubernetes" {
  config_path    = "~/.kube/config"
  config_context = "minikube"
}

resource "kubernetes_config_map" "redis-conf" {
  metadata {
    name = "redis-conf"
  }

  data = {
    protected-mode = "no"
    daemonize      = "no"
  }
}
