using Domain.Identidade.Enums;
using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.Identidade.ValueObjects
{
    [ValueObject]
    public record DocumentoIdentificadorUsuario
    {
        private readonly string _valor = string.Empty;
        private readonly TipoDocumentoIdentificadorUsuarioEnum _tipoDocumento;

        // Construtor sem parâmetro para EF Core
        private DocumentoIdentificadorUsuario() { }

        public DocumentoIdentificadorUsuario(string documento)
        {
            var documentoLimpo = LimparDocumento(documento);
            
            if (ValidarCpf(documentoLimpo))
            {
                _valor = documentoLimpo;
                _tipoDocumento = TipoDocumentoIdentificadorUsuarioEnum.CPF;
            }
            else if (ValidarCnpj(documentoLimpo))
            {
                _valor = documentoLimpo;
                _tipoDocumento = TipoDocumentoIdentificadorUsuarioEnum.CNPJ;
            }
            else
            {
                throw new DomainException("Documento de identificação de usuário inválido", ErrorType.InvalidInput);
            }
        }

        public string Valor => _valor;
        public TipoDocumentoIdentificadorUsuarioEnum TipoDocumento => _tipoDocumento;

        private static bool ValidarCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = LimparDocumento(cpf);

            if (cpf.Length != 11)
                return false;

            if (cpf.All(c => c == cpf[0]))
                return false;

            int[] multiplier1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplier2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf[..9];
            int sum = 0;

            for (int i = 0; i < 9; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multiplier1[i];

            int remainder = sum % 11;
            remainder = remainder < 2 ? 0 : 11 - remainder;

            string digit = remainder.ToString();
            tempCpf += digit;

            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multiplier2[i];

            remainder = sum % 11;
            remainder = remainder < 2 ? 0 : 11 - remainder;

            digit += remainder.ToString();

            return cpf.EndsWith(digit);
        }

        private static bool ValidarCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            cnpj = LimparDocumento(cnpj);

            if (cnpj.Length != 14)
                return false;

            if (cnpj.All(c => c == cnpj[0]))
                return false;

            // Validação do primeiro dígito verificador
            int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCnpj = cnpj[..12];
            int sum = 0;

            for (int i = 0; i < 12; i++)
                sum += int.Parse(tempCnpj[i].ToString()) * multiplier1[i];

            int remainder = sum % 11;
            remainder = remainder < 2 ? 0 : 11 - remainder;

            string digit = remainder.ToString();
            tempCnpj += digit;

            // Validação do segundo dígito verificador
            int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            sum = 0;

            for (int i = 0; i < 13; i++)
                sum += int.Parse(tempCnpj[i].ToString()) * multiplier2[i];

            remainder = sum % 11;
            remainder = remainder < 2 ? 0 : 11 - remainder;

            digit += remainder.ToString();

            return cnpj.EndsWith(digit);
        }

        private static string LimparDocumento(string documento)
        {
            return string.Join("", documento.Where(char.IsDigit));
        }
    }
}