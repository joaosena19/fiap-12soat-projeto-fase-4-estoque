using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Authentication
{
    /// <summary>
    /// Interface para validação de assinatura HMAC de webhooks
    /// </summary>
    public interface IHmacValidationService
    {
        /// <summary>
        /// Valida a assinatura HMAC de um webhook
        /// </summary>
        /// <param name="payload">Payload da requisição</param>
        /// <param name="signature">Assinatura recebida no header</param>
        /// <returns>True se a assinatura for válida</returns>
        bool ValidateSignature(string payload, string signature);
    }

    /// <summary>
    /// Serviço para validação de assinatura HMAC de webhooks
    /// </summary>
    public class HmacValidationService : IHmacValidationService
    {
        private readonly string _secretKey;

        public HmacValidationService(IConfiguration configuration)
        {
            _secretKey = configuration["Webhook:HmacSecret"] ?? throw new InvalidOperationException("Webhook HMAC Secret não configurado");
        }

        public bool ValidateSignature(string payload, string signature)
        {
            if (string.IsNullOrEmpty(payload) || string.IsNullOrEmpty(signature))
                return false;

            try
            {
                // Remove o prefixo "sha256=" se presente
                if (signature.StartsWith("sha256="))
                    signature = signature[7..];

                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                var computedSignature = Convert.ToHexString(computedHash).ToLowerInvariant();

                return computedSignature.Equals(signature, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}