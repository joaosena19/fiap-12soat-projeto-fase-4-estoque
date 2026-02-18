[![Deploy](https://github.com/joaosena19/fiap-12soat-projeto-fase-4-estoque/actions/workflows/deploy.yaml/badge.svg)](https://github.com/joaosena19/fiap-12soat-projeto-fase-4-estoque/actions/workflows/deploy.yaml)

# Identificação

Aluno: João Pedro Sena Dainese  
Registro FIAP: RM365182  

Turma 12SOAT - Software Architecture  
Grupo individual  
Grupo 13  

Discord: joaodainese  
Email: joaosenadainese@gmail.com  

## Sobre este Repositório

Este repositório contém apenas parte do projeto completo da Fase 4. Para visualizar a documentação completa, diagramas de arquitetura, e todos os componentes do projeto, acesse: [Documentação Completa - Fase 4](https://github.com/joaosena19/fiap-12soat-projeto-fase-4-documentacao)

## Descrição

Microsserviço de Estoque em .NET, responsável pelo gerenciamento de itens de estoque utilizados nas ordens de serviço. Participa da SAGA coreografada como consumidor de eventos de redução de estoque. Implementado com Clean Architecture, PostgreSQL no Amazon RDS e Entity Framework Core. Executado em Kubernetes (EKS) com HPA.

## Tecnologias Utilizadas

- **.NET** - Runtime e framework web
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados (Amazon RDS)
- **MassTransit + Amazon SQS** - Mensageria assíncrona (SAGA)
- **JWT Authentication** - Autenticação via Forward Token
- **Swagger** - Documentação API
- **Docker** - Containerização
- **Kubernetes** - Orquestração (Amazon EKS)
- **Terraform** - Provisionamento do banco de dados
- **SonarCloud** - Análise estática de qualidade

## Documentação da API

Para consultar a documentação completa dos endpoints da API, acesse: [Documentação de Endpoints - Estoque](https://github.com/joaosena19/fiap-12soat-projeto-fase-4-documentacao/blob/main/10.%20Endpoints/3_endpoints_estoque.md)
