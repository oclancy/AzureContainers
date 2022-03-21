variable "region" {
  type    = string
  default = "eu-west-2"
}

variable "account_id" {
  type    = string
}

variable "backend" {
  type = object({
    bucket = string
    key      = string

  })
}
