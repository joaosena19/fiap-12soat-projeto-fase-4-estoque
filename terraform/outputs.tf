output "db_instance_id" {
  description = "ID da instância PostgreSQL RDS do Estoque"
  value       = aws_db_instance.estoque_postgres.id
}

output "db_instance_endpoint" {
  description = "Endpoint da instância PostgreSQL RDS do Estoque"
  value       = aws_db_instance.estoque_postgres.endpoint
}

output "db_instance_address" {
  description = "Endereço (host) da instância PostgreSQL RDS do Estoque"
  value       = aws_db_instance.estoque_postgres.address
}

output "db_instance_port" {
  description = "Porta da instância PostgreSQL RDS do Estoque"
  value       = aws_db_instance.estoque_postgres.port
}

output "db_name" {
  description = "Nome do banco de dados do Estoque"
  value       = aws_db_instance.estoque_postgres.db_name
}

output "db_username" {
  description = "Username master do banco de dados do Estoque"
  value       = aws_db_instance.estoque_postgres.username
  sensitive   = true
}

output "db_security_group_id" {
  description = "ID do Security Group do PostgreSQL do Estoque"
  value       = aws_security_group.estoque_db_sg.id
}

output "db_subnet_group_name" {
  description = "Nome do DB Subnet Group do Estoque"
  value       = aws_db_subnet_group.estoque_subnet_group.name
}

# Connection string para uso nos ConfigMaps/Secrets do K8s
output "db_connection_string" {
  description = "Connection string do banco de dados do Estoque (sem senha)"
  value       = "Host=${aws_db_instance.estoque_postgres.address};Port=${aws_db_instance.estoque_postgres.port};Database=${aws_db_instance.estoque_postgres.db_name};Username=${aws_db_instance.estoque_postgres.username}"
  sensitive   = true
}
