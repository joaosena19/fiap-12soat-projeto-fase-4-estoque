using Bogus;
using Domain.Cadastros.Enums;
using VeiculoAggregate = Domain.Cadastros.Aggregates.Veiculo;

namespace Tests.Application.SharedHelpers.AggregateBuilders
{
    public class VeiculoBuilder
    {
        private Guid _clienteId;
        private string _placa;
        private string _modelo;
        private string _marca;
        private string _cor;
        private int _ano;
        private TipoVeiculoEnum _tipoVeiculo;
        private readonly Faker _faker = new Faker("pt_BR");

        public VeiculoBuilder()
        {
            _clienteId = Guid.NewGuid();
            _placa = _faker.Random.Replace("???-####");
            _modelo = _faker.Vehicle.Model();
            _marca = _faker.Vehicle.Manufacturer();
            _cor = _faker.Commerce.Color();
            _ano = _faker.Random.Int(2000, DateTime.Now.Year);
            _tipoVeiculo = _faker.Random.Enum<TipoVeiculoEnum>();
        }

        public VeiculoBuilder ComClienteId(Guid clienteId)
        {
            _clienteId = clienteId;
            return this;
        }

        public VeiculoBuilder ComCliente(Guid clienteId)
        {
            return ComClienteId(clienteId);
        }

        public VeiculoBuilder ComPlaca(string placa)
        {
            _placa = placa;
            return this;
        }

        public VeiculoBuilder ComModelo(string modelo)
        {
            _modelo = modelo;
            return this;
        }

        public VeiculoBuilder ComMarca(string marca)
        {
            _marca = marca;
            return this;
        }

        public VeiculoBuilder ComCor(string cor)
        {
            _cor = cor;
            return this;
        }

        public VeiculoBuilder ComAno(int ano)
        {
            _ano = ano;
            return this;
        }

        public VeiculoBuilder ComTipo(TipoVeiculoEnum tipo)
        {
            _tipoVeiculo = tipo;
            return this;
        }

        public VeiculoAggregate Build()
        {
            return VeiculoAggregate.Criar(_clienteId, _placa, _modelo, _marca, _cor, _ano, _tipoVeiculo);
        }
    }
}