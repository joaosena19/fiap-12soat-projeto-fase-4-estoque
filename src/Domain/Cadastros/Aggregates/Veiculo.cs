using Domain.Cadastros.Enums;
using Domain.Cadastros.ValueObjects.Veiculo;
using Shared.Attributes;
using UUIDNext;

namespace Domain.Cadastros.Aggregates
{
    [AggregateRoot]
    public class Veiculo
    {
        public Guid Id { get; private set; }
        public Guid ClienteId { get; private set; }
        public Placa Placa { get; private set; } = null!;
        public Modelo Modelo { get; private set; } = null!;
        public Marca Marca { get; private set; } = null!;
        public Cor Cor { get; private set; } = null!;
        public Ano Ano { get; private set; } = null!;
        public TipoVeiculo TipoVeiculo { get; private set; } = null!;

        // Contrutor sem parâmetro para EF Core
        private Veiculo() { }

        private Veiculo(Guid id, Guid clienteId, Placa placa, Modelo modelo, Marca marca, Cor cor, Ano ano, TipoVeiculo tipoVeiculo)
        {
            Id = id;
            ClienteId = clienteId;
            Placa = placa;
            Modelo = modelo;
            Marca = marca;
            Cor = cor;
            Ano = ano;
            TipoVeiculo = tipoVeiculo;
        }

        public static Veiculo Criar(Guid clienteId, string placa, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo)
        {
            return new Veiculo(
                Uuid.NewSequential(),
                clienteId,
                new Placa(placa),
                new Modelo(modelo),
                new Marca(marca),
                new Cor(cor),
                new Ano(ano),
                new TipoVeiculo(tipoVeiculo)
            );
        }

        public void Atualizar(string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo)
        {
            Modelo = new Modelo(modelo);
            Marca = new Marca(marca);
            Cor = new Cor(cor);
            Ano = new Ano(ano);
            TipoVeiculo = new TipoVeiculo(tipoVeiculo);
        }
    }
}
