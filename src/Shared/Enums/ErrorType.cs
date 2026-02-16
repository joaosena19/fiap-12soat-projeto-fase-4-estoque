namespace Shared.Enums
{
    /// <summary>
    /// Tipos customizados de erro para decoupling da camada de apresentação
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// Entrada inválida (validação de dados de entrada)
        /// </summary>
        InvalidInput,

        /// <summary>
        /// Recurso não encontrado (quando buscado diretamente, e.g.: por ID na rota)
        /// </summary>
        ResourceNotFound,

        /// <summary>
        /// Referência não encontrada (quando buscado dentro de outro fluxo, e.g.: por ID no corpo da requisição)
        /// </summary>
        ReferenceNotFound,

        /// <summary>
        /// Regra de domínio violada (regras de negócio)
        /// </summary>
        DomainRuleBroken,

        /// <summary>
        /// Conflito de estado (ex: recurso já existe, estado inválido)
        /// </summary>
        Conflict,

        /// <summary>
        /// Não autorizado
        /// </summary>
        Unauthorized,

        /// <summary>
        /// Acesso negado (usuário autenticado mas sem permissão)
        /// </summary>
        NotAllowed,

        /// <summary>
        /// Erro inesperado (erro padrão)
        /// </summary>
        UnexpectedError
    }
}
