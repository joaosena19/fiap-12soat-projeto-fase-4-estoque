namespace API.Configurations
{
    /// <summary>
    /// Configuração de cabeçalhos de segurança
    /// </summary>
    public static class SecurityConfiguration
    {
        /// <summary>
        /// Configura os cabeçalhos de segurança da aplicação
        /// </summary>
        /// <param name="app">Aplicação web</param>
        /// <returns>Aplicação web configurada</returns>
        public static IApplicationBuilder UseSecurityHeadersConfiguration(this IApplicationBuilder app)
        {
            app.UseSecurityHeaders(policy =>
            {
                policy.AddDefaultSecurityHeaders();
                policy.AddContentSecurityPolicy(builder =>
                {
                    // Configura o CSP. Só permite scripts do próprio domínio.
                    builder.AddScriptSrc().Self();
                });
            });

            return app;
        }
    }
}
