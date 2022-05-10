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
          #command = ["redis-server", "/etc/redis/redis.conf"]

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
          #command = ["redis-server", "/etc/redis/redis.conf"]

        }
      }

    }
  }
}
