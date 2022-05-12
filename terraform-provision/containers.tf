resource "kubernetes_deployment" "console-app1" {
  metadata {
    name = "${var.product}-console-app-1"
    labels = {
      app = "console-app-1-deploy"
    }
  }

  spec {
    replicas = 1

    selector {
      match_labels = {
        app = "console-app-1"
      }
    }

    template {
      metadata {
        labels = {
          app = "console-app-1"
        }
      }

      spec {
        container {
          image = "firmus5oftware/consoleapp1:latest"
          name  = "conoleapp1"
          env {
            name = "RedisOptions__IpAddress"
            value_from {

              config_map_key_ref {
                name = kubernetes_config_map.containers_config.metadata[0].name # The ConfigMap this value comes from.
                key  = "redisIp"                                                # The key to fetch.  
              }
            }
          }

          volume_mount {
            name       = "appsettings"
            mount_path = "/app/settings/"
          }
        }
        volume {
          name = "appsettings"
          config_map {
            name = kubernetes_config_map.containers_config.metadata[0].name
            items {
              key  = "appsettings"
              path = "appsettings.json"
            }

          }
        }
      }

    }
  }
}

resource "kubernetes_deployment" "console-app2" {
  metadata {
    name = "${var.product}-console-app-2"
    labels = {
      app = "console-app-2-deploy"
    }
  }

  spec {
    replicas = 1

    selector {
      match_labels = {
        app = "console-app-2"
      }
    }

    template {
      metadata {
        labels = {
          app = "console-app-2"
        }
      }

      spec {
        container {
          image = "firmus5oftware/consoleapp2:latest"
          name  = "conoleapp2"
          env {
            name = "RedisOptions__IpAddress"
            value_from {

              config_map_key_ref {
                name = kubernetes_config_map.containers_config.metadata[0].name # The ConfigMap this value comes from.
                key  = "redisIp"                                                # The key to fetch.  
              }
            }
          }
          volume_mount {
            name       = "appsettings"
            mount_path = "/app/settings"
          }
        }
        volume {
          name = "appsettings"
          config_map {
            name = kubernetes_config_map.containers_config.metadata[0].name
            items {
              key  = "appsettings"
              path = "appsettings.json"
            }

          }
        }
      }

    }
  }
}
