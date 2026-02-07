# Data source para obter informações da VPC e subnets do terraform da infraestrutura
data "terraform_remote_state" "infra" {
  backend = "s3"
  config = {
    bucket = var.terraform_state_bucket
    key    = var.infra_terraform_state_key
    region = var.aws_region
  }
}
