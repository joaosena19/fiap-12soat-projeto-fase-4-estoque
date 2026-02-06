using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;

namespace Tests.Integration
{
    public static class TestHmacExtensions
    {
        /// <summary>
        /// Cria um cliente HTTP configurado para usar HMAC de teste
        /// </summary>
        public static HttpClient CreateHmacClient<TEntryPoint>(this WebApplicationFactory<TEntryPoint> factory)
            where TEntryPoint : class
        {
            // O TestWebApplicationFactory já configura o IConfiguration com secret de teste
            // O serviço HMAC real será usado com o secret de teste
            return factory.CreateClient();
        }

        /// <summary>
        /// Envia uma requisição POST com HMAC válido para endpoints webhook
        /// </summary>
        public static async Task<HttpResponseMessage> PostAsJsonWithHmacAsync<T>(
            this HttpClient client,
            string requestUri,
            T value)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            // Serializar o objeto para JSON
            var json = System.Text.Json.JsonSerializer.Serialize(value, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });

            // Gerar assinatura HMAC
            var signature = TestHmacUtils.GenerateHmacSignature(json);

            // Configurar headers e body
            request.Headers.Add("X-Signature", signature);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return await client.SendAsync(request);
        }
    }
}