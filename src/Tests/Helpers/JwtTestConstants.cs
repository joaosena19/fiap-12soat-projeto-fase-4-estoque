namespace Tests.Helpers
{
    /// <summary>
    /// Constantes centralizadas para configuração JWT em testes
    /// </summary>
    public static class JwtTestConstants
    {
        /// <summary>
        /// Chave JWT para testes (256 bits mínimo para HMAC)
        /// </summary>
        public const string Key = "test-jwt-key-for-integration-tests";

        /// <summary>
        /// Issuer JWT para testes
        /// </summary>
        public const string Issuer = "TestOficinaMecanicaApi";

        /// <summary>
        /// Audience JWT para testes
        /// </summary>
        public const string Audience = "TestAuthorizedServices";
    }
}