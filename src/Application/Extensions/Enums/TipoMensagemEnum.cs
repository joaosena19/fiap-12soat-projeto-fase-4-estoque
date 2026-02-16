namespace Application.Extensions.Enums;

/// <summary>
/// Tipo de operação de mensageria (consumo ou publicação).
/// </summary>
public enum TipoMensagemEnum
{
    /// <summary>
    /// Mensagem sendo consumida de uma fila.
    /// </summary>
    Consumo,

    /// <summary>
    /// Mensagem sendo publicada para uma fila.
    /// </summary>
    Publicacao
}
