namespace Tests.Helpers
{
    public static class DocumentoHelper
    {
        public static string GerarCpfValido()
        {
            int soma = 0, resto = 0;
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            Random rnd = new Random();
            string semente = rnd.Next(100000000, 999999999).ToString();
            for (int i = 0; i < 9; i++)
                soma += int.Parse(semente[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            semente = semente + resto;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(semente[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            semente = semente + resto;
            return semente;
        }

        public static string GerarCpfInvalido()
        {
            // Gera um CPF válido e altera o último dígito para garantir que fique inválido
            var cpfValido = GerarCpfValido();
            // Troca o último dígito
            char ultimo = cpfValido.Last();
            char novoUltimo = ultimo != '9' ? (char)(ultimo + 1) : '0';
            return cpfValido.Substring(0, cpfValido.Length - 1) + novoUltimo;
        }

        public static string GerarCnpjValido()
        {
            int soma = 0, resto = 0;
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            Random rnd = new Random();
            string semente = rnd.Next(10000000, 99999999).ToString() + "0001";
            for (int i = 0; i < 12; i++)
                soma += int.Parse(semente[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            semente = semente + resto;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(semente[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            semente = semente + resto;
            return semente;
        }

        public static string GerarCnpjInvalido()
        {
            var cnpjValido = GerarCnpjValido();
            // Troca o último dígito
            char ultimo = cnpjValido.Last();
            char novoUltimo = ultimo != '9' ? (char)(ultimo + 1) : '0';
            return cnpjValido.Substring(0, cnpjValido.Length - 1) + novoUltimo;
        }
    }
}