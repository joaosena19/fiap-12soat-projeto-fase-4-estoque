terraform {
  backend "s3" {
    bucket  = "fiap-12soat-fase3-joao-dainese"
    key     = "fase4/estoque-database/terraform.tfstate"
    region  = "us-east-1"
    encrypt = true
  }
}
