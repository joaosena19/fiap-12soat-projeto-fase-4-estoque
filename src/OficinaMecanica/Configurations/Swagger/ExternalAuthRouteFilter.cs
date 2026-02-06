using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Configurations.Swagger;

/// <summary>
/// Filtro personalizado para adicionar rotas de autenticação externa (AWS Lambda) no Swagger
/// </summary>
public class ExternalAuthRouteFilter : IDocumentFilter
{
    /// <summary>
    /// Aplica modificações no documento OpenAPI/Swagger
    /// </summary>
    /// <param name="swaggerDoc">Documento OpenAPI a ser modificado</param>
    /// <param name="context">Contexto do filtro de documento</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var pathItem = new OpenApiPathItem();
        
        var operation = new OpenApiOperation
        {
            Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Autenticação" } },
            Summary = "Realiza o login e retorna o Token JWT (Processado via AWS Lambda)",
            Description = "Este endpoint é interceptado pelo API Gateway e processado por uma AWS Lambda externa. " +
                         "O endpoint não é implementado diretamente na aplicação, mas é documentado aqui para referência da API completa.",
            RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(LoginRequestDto), context.SchemaRepository)
                    }
                },
                Required = true,
                Description = "Dados para autenticação do usuário"
            },
            Responses = new OpenApiResponses()
        };

        // Resposta de sucesso
        operation.Responses.Add("200", new OpenApiResponse 
        { 
            Description = "Token gerado com sucesso",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = context.SchemaGenerator.GenerateSchema(typeof(LoginResponseDto), context.SchemaRepository)
                }
            }
        });

        // Resposta de erro - Usuário não encontrado
        operation.Responses.Add("404", new OpenApiResponse 
        { 
            Description = "Usuário não encontrado"
        });

        // Resposta de erro - Dados inválidos
        operation.Responses.Add("400", new OpenApiResponse 
        { 
            Description = "Dados de entrada inválidos"
        });

        // Resposta de erro interno
        operation.Responses.Add("500", new OpenApiResponse 
        { 
            Description = "Erro interno do servidor"
        });

        // Adiciona o verbo POST
        pathItem.AddOperation(OperationType.Post, operation);

        // Adiciona a rota no documento final
        swaggerDoc.Paths.Add("/auth/authenticate", pathItem);
    }
}