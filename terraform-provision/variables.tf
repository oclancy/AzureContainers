variable "product" {
  type    = string
  default = "containersfun"
}

variable "region" {
  type    = string
  default = "eu-west-2"
}

variable "account_id" {
  type    = string
  default = ""
}

variable "environment" {
  description = "the name of your environment, e.g. \"prod\""
  default     = "prod"
}

# variable "application-secrets" {
#   description = "A map of secrets that is passed into the application. Formatted like ENV_VAR = VALUE"
#   type        = map(any)
# }

