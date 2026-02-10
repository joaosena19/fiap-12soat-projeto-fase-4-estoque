using Moq;
using Application.Contracts.Presenters;
using Shared.Enums;
using System.Globalization;
using System.Text;

namespace Tests.Application
{
    public static class BasePresenterMockExtensions
    {
        public static void DeveTerApresentadoSucessoComQualquerObjeto<TPresenter, TSucesso>(this Mock<TPresenter> mock)
            where TPresenter : class, IBasePresenter<TSucesso>
        {
            mock.Verify(p => p.ApresentarSucesso(It.IsAny<TSucesso>()), Times.Once,
                "Era esperado que o método ApresentarSucesso fosse chamado exatamente uma vez com um objeto de sucesso.");
        }

        public static void DeveTerApresentadoSucessoComQualquerObjeto<TPresenter, TSucesso>(this Mock<TPresenter> mock, IEnumerable<TSucesso> colecaoEsperada)
            where TPresenter : class, IBasePresenter<IEnumerable<TSucesso>>
        {
            mock.Verify(p => p.ApresentarSucesso(It.Is<IEnumerable<TSucesso>>(colecaoReal => colecaoReal != null && colecaoReal.SequenceEqual(colecaoEsperada))), Times.Once,
                "Era esperado que o método ApresentarSucesso fosse chamado exatamente uma vez com a coleção fornecida.");
        }

        public static void DeveTerApresentadoSucesso<TPresenter, TSucesso>(this Mock<TPresenter> mock, TSucesso objeto)
                    where TPresenter : class, IBasePresenter<TSucesso>
        {
            mock.Verify(p => p.ApresentarSucesso(It.Is<TSucesso>(o => Equals(o, objeto))), Times.Once,
                "Era esperado que o método ApresentarSucesso fosse chamado exatamente uma vez com o objeto fornecido.");
        }

        // Override para lista
        public static void DeveTerApresentadoSucesso<TPresenter, TSucesso>(this Mock<TPresenter> mock, IEnumerable<TSucesso> colecaoEsperada)
            where TPresenter : class, IBasePresenter<IEnumerable<TSucesso>>
        {
            mock.Verify(p => p.ApresentarSucesso(It.Is<IEnumerable<TSucesso>>(colecaoReal => colecaoReal != null && colecaoReal.SequenceEqual(colecaoEsperada))), Times.Once,
                "Era esperado que o método ApresentarSucesso fosse chamado exatamente uma vez com a coleção fornecida.");
        }

        public static void DeveTerApresentadoErro<TPresenter, TSucesso>(this Mock<TPresenter> mock, string mensagem, ErrorType errorType)
            where TPresenter : class, IBasePresenter<TSucesso>
        {
            mock.Verify(
                p => p.ApresentarErro(
                    It.Is<string>(mensagemReal => ContemTextoNormalizado(mensagemReal, mensagem)),
                    errorType
                ),
                Times.Once,
                $"Era esperado que o método ApresentarErro fosse chamado exatamente uma vez com a mensagem '{mensagem}' (contains, ignorando case, espaços e acentos) e tipo '{errorType}'."
            );
        }

        public static void NaoDeveTerApresentadoSucesso<TPresenter, TSucesso>(this Mock<TPresenter> mock)
            where TPresenter : class, IBasePresenter<TSucesso>
        {
            mock.Verify(p => p.ApresentarSucesso(It.IsAny<TSucesso>()), Times.Never,
                "O método ApresentarSucesso não deveria ter sido chamado.");
        }

        public static void NaoDeveTerApresentadoErro<TPresenter, TSucesso>(this Mock<TPresenter> mock)
            where TPresenter : class, IBasePresenter<TSucesso>
        {
            mock.Verify(p => p.ApresentarErro(It.IsAny<string>(), It.IsAny<ErrorType>()), Times.Never,
                "O método ApresentarErro não deveria ter sido chamado.");
        }

        // COMENTADO - Overrides para IOperacaoOrdemServicoPresenter (não pertence ao bounded context de Estoque)
        /*
        public static void DeveTerApresentadoSucesso(this Mock<IOperacaoOrdemServicoPresenter> mock)
        {
            mock.Verify(p => p.ApresentarSucesso(), Times.Once,
                "Era esperado que o método ApresentarSucesso fosse chamado exatamente uma vez.");
        }

        public static void NaoDeveTerApresentadoSucesso(this Mock<IOperacaoOrdemServicoPresenter> mock)
        {
            mock.Verify(p => p.ApresentarSucesso(), Times.Never,
                "O método ApresentarSucesso não deveria ter sido chamado.");
        }

        public static void DeveTerApresentadoErro(this Mock<IOperacaoOrdemServicoPresenter> mock, string mensagem, ErrorType errorType)
        {
            mock.Verify(
                p => p.ApresentarErro(
                    It.Is<string>(mensagemReal => ContemTextoNormalizado(mensagemReal, mensagem)),
                    errorType
                ),
                Times.Once,
                $"Era esperado que o método ApresentarErro fosse chamado exatamente uma vez com a mensagem '{mensagem}' (contains, ignorando case, espaços e acentos) e tipo '{errorType}'."
            );
        }

        public static void DeveTerApresentadoErroComTipo(this Mock<IOperacaoOrdemServicoPresenter> mock, ErrorType errorType)
        {
            mock.Verify(p => p.ApresentarErro(It.IsAny<string>(), errorType), Times.Once,
                $"Era esperado que o método ApresentarErro fosse chamado exatamente uma vez com qualquer mensagem e tipo '{errorType}'.");
        }

        public static void NaoDeveTerApresentadoErro(this Mock<IOperacaoOrdemServicoPresenter> mock)
        {
            mock.Verify(p => p.ApresentarErro(It.IsAny<string>(), It.IsAny<ErrorType>()), Times.Never,
                "O método ApresentarErro não deveria ter sido chamado.");
        }
        */

        // Helpers de normalização/comparação para mensagens
        private static bool ContemTextoNormalizado(string textoCompleto, string textoProcurado)
        {
            string textoCompletoNormalizado = NormalizarTextoParaComparacao(textoCompleto);
            string textoProcuradoNormalizado = NormalizarTextoParaComparacao(textoProcurado);

            return textoCompletoNormalizado.Contains(textoProcuradoNormalizado);
        }

        private static string NormalizarTextoParaComparacao(string textoDeEntrada)
        {
            if (string.IsNullOrEmpty(textoDeEntrada))
            {
                return string.Empty;
            }

            string textoDecomposto = textoDeEntrada.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(textoDecomposto.Length);

            foreach (char caractere in textoDecomposto)
            {
                UnicodeCategory categoriaUnicode = CharUnicodeInfo.GetUnicodeCategory(caractere);

                if (categoriaUnicode != UnicodeCategory.NonSpacingMark && !char.IsWhiteSpace(caractere))
                    stringBuilder.Append(caractere);
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }

    }
}
