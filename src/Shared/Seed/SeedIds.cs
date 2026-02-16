namespace Shared.Seed;

public static class SeedIds
{
    public static class Veiculos
    {
        public static readonly Guid Abc1234 = Guid.Parse("3f8a2d3b-0d8b-4a3f-9b1e-7b65e6d2a901");
        public static readonly Guid Xyz5678 = Guid.Parse("0d2c5f44-6a50-4f8e-8d7a-0d6c7b0d1b2c");
        public static readonly Guid Def9012 = Guid.Parse("9b6d2a10-6a2f-4f7a-9e1b-2a3f0d8b3f8a");
    }
    
    public static class Servicos
    {
        public static readonly Guid TrocaDeOleo = Guid.Parse("1a111111-1111-1111-1111-111111111111");
        public static readonly Guid AlinhamentoBalanceamento = Guid.Parse("2b222222-2222-2222-2222-222222222222");
        public static readonly Guid RevisaoCompleta = Guid.Parse("3c333333-3333-3333-3333-333333333333");
    }
    
    public static class ItensEstoque
    {
        public static readonly Guid OleoMotor5w30 = Guid.Parse("4d444444-4444-4444-4444-444444444444");
        public static readonly Guid FiltroDeOleo = Guid.Parse("5e555555-5555-5555-5555-555555555555");
        public static readonly Guid PastilhaDeFreioDianteira = Guid.Parse("6f666666-6666-6666-6666-666666666666");
    }
}
