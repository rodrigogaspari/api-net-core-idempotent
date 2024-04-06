using FluentAssertions;
using ApiIdempotent.Application.Commands.Requests;
using ApiIdempotent.Application.Queries.Responses;
using ApiIdempotent.IntegrationTests.Factories;
using System.Net;
using System.Net.Http.Json;

namespace ApiIdempotent.IntegrationTests.Controllers
{
    [Collection("Database")]
    public class ContaCorrenteControllerTests : IClassFixture<ApiIdempotentFactory>
    {
        private readonly ApiIdempotentFactory _factory;

        public ContaCorrenteControllerTests(ApiIdempotentFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateMovimentacao_ShouldReturn_OK()
        {
            //Arrange
            var client = _factory.CreateClient();
            var guid = Guid.NewGuid().ToString();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("IdempotencyKey", guid);


            //Act
            var request = new CriarMovimentacaoRequest()
            {
                IdRequisicao = Guid.NewGuid().ToString(),
                TipoMovimento = "C",
                Valor = 50
            };
            var idContaCorrente = "FA99D033-7067-ED11-96C6-7C5DFA4A16C9";
            var response = await client.PostAsJsonAsync($"api/v1/ContaCorrente/{idContaCorrente}/movimentacao", request);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task CreateMovimentacaoAndGetSaldo_ShouldReturn_OK()
        {
            //Arrange
            var client = _factory.CreateClient();
            var guid = Guid.NewGuid().ToString();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("IdempotencyKey", guid);

            //Act
            var request = new CriarMovimentacaoRequest()
            {
                IdRequisicao = Guid.NewGuid().ToString(),
                TipoMovimento = "C",
                Valor = 150
            };
            var idContaCorrente = "382D323D-7067-ED11-8866-7D5DFA4A16C9";

            await client.PostAsJsonAsync($"api/v1/ContaCorrente/{idContaCorrente}/movimentacao", request);

            var response = await client.GetFromJsonAsync<IEnumerable<ConsultaSaldoResponse>>($"api/v1/ContaCorrente/{idContaCorrente}/saldo");

            //Assert
            Assert.NotNull(response);
            Assert.Single(response);
            Assert.Equal(150, response.First().Saldo);
        }


        [Fact]
        public async Task CreateDuplicationMovimentacaoAndGetVerificaSaldo_ShouldReturn_ConsistentBalance()
        {
            // Arrange
            var client = _factory.CreateClient();
            var guid = Guid.NewGuid().ToString();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("IdempotencyKey", guid);

            // Act
            var request = new CriarMovimentacaoRequest()
            {
                IdRequisicao = Guid.NewGuid().ToString(),
                TipoMovimento = "C",
                Valor = 150
            };
            var idContaCorrente = "B6BAFC09 -6967-ED11-A567-055DFA4A16C9";

            var response1 = await client.PostAsJsonAsync($"api/v1/ContaCorrente/{idContaCorrente}/movimentacao", request);
            var response2 = await client.PostAsJsonAsync($"api/v1/ContaCorrente/{idContaCorrente}/movimentacao", request);

            var responseSaldo = await client.GetFromJsonAsync<IEnumerable<ConsultaSaldoResponse>>($"api/v1/ContaCorrente/{idContaCorrente}/saldo");


            // Assert
            var content1 = await response1.Content.ReadAsStringAsync();
            var content2 = await response2.Content.ReadAsStringAsync();

            response1.StatusCode.Should().Be(HttpStatusCode.OK);
            response2.StatusCode.Should().Be(HttpStatusCode.OK);

            responseSaldo?.First()?.Saldo.Should().Be(150);
        }
    }
}
