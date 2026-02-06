using Application.OrdemServico.Dtos;
using Bogus;
using Domain.Cadastros.Enums;
using Tests.Helpers;

namespace Tests.Application.OrdemServico.Helpers
{
    public class CriarOrdemServicoCompletaDtoBuilder
    {
        private string _nomeCliente;
        private string _documentoCliente;
        private string _placaVeiculo;
        private string _modeloVeiculo;
        private string _marcaVeiculo;
        private string _corVeiculo;
        private int _anoVeiculo;
        private TipoVeiculoEnum _tipoVeiculo;
        private List<Guid>? _servicosIds = null;
        private List<ItemDto>? _itens = null;
        private readonly Faker _faker = new Faker("pt_BR");

        public CriarOrdemServicoCompletaDtoBuilder()
        {
            _nomeCliente = _faker.Person.FullName;
            _documentoCliente = DocumentoHelper.GerarCpfValido();
            _placaVeiculo = _faker.Random.Replace("???-####");
            _modeloVeiculo = _faker.Vehicle.Model();
            _marcaVeiculo = _faker.Vehicle.Manufacturer();
            _corVeiculo = _faker.Commerce.Color();
            _anoVeiculo = _faker.Random.Int(2000, DateTime.Now.Year);
            _tipoVeiculo = _faker.Random.Enum<TipoVeiculoEnum>();
        }

        public CriarOrdemServicoCompletaDtoBuilder ComCliente(string nome, string documento)
        {
            _nomeCliente = nome;
            _documentoCliente = documento;
            return this;
        }

        public CriarOrdemServicoCompletaDtoBuilder ComVeiculo(string placa, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipo)
        {
            _placaVeiculo = placa;
            _modeloVeiculo = modelo;
            _marcaVeiculo = marca;
            _corVeiculo = cor;
            _anoVeiculo = ano;
            _tipoVeiculo = tipo;
            return this;
        }

        public CriarOrdemServicoCompletaDtoBuilder ComServicos(params Guid[] servicosIds)
        {
            _servicosIds = servicosIds.ToList();
            return this;
        }

        public CriarOrdemServicoCompletaDtoBuilder ComItens(params (Guid itemId, int quantidade)[] itens)
        {
            _itens = itens.Select(i => new ItemDto { ItemEstoqueId = i.itemId, Quantidade = i.quantidade }).ToList();
            return this;
        }

        public CriarOrdemServicoCompletaDtoBuilder SemServicos()
        {
            _servicosIds = null;
            return this;
        }

        public CriarOrdemServicoCompletaDtoBuilder SemItens()
        {
            _itens = null;
            return this;
        }

        public CriarOrdemServicoCompletaDto Build()
        {
            return new CriarOrdemServicoCompletaDto
            {
                Cliente = new ClienteDto
                {
                    Nome = _nomeCliente,
                    DocumentoIdentificador = _documentoCliente
                },
                Veiculo = new VeiculoDto
                {
                    Placa = _placaVeiculo,
                    Modelo = _modeloVeiculo,
                    Marca = _marcaVeiculo,
                    Cor = _corVeiculo,
                    Ano = _anoVeiculo,
                    TipoVeiculo = _tipoVeiculo
                },
                ServicosIds = _servicosIds,
                Itens = _itens
            };
        }
    }
}