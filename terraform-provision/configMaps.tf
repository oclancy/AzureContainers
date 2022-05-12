
resource "kubernetes_config_map" "containers_config" {
  metadata {
    name = "appsettings"
  }

  data = {
    "appsettings" = "${file("${path.module}/../ConsoleApp2/appsettings.json")}",
    "redisIp"     = "redis"
  }
}
