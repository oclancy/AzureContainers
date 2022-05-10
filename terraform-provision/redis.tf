resource "kubernetes_deployment" "redis" {
  metadata {
    name = "${var.product}-redis"
    labels = {
      test    = "${var.product}",
      product = var.product
    }
  }

  spec {
    replicas = 1

    selector {
      match_labels = {
        product = var.product
      }
    }

    template {
      metadata {
        labels = {
          product = var.product
        }
      }

      spec {
        container {
          image   = "redis:latest"
          name    = "redis"
          command = ["redis-server", "/etc/redis/redis.conf"]
          port {
            container_port = 6379
          }
          volume_mount {
            name       = "redis-conf"
            mount_path = "/etc/redis/redis.conf"
          }
        }
        volume {
          name = "redis-conf"
          config_map {
            name = kubernetes_config_map.redis-conf.metadata[0].name
          }
        }

      }

    }
  }
}

resource "kubernetes_service" "redis" {
  metadata {
    name = "redis"
  }
  spec {
    selector = {
      product = var.product
    }
    port {
      port        = 6379
      target_port = 6379
    }
    type = "NodePort"
  }
}
