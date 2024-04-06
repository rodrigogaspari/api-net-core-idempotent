using IdempotentAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using ApiIdempotent.Application.Abstractions;
using ApiIdempotent.Application.Commands.Requests;
using ApiIdempotent.Application.Queries.Responses;
using ApiIdempotent.Infrastructure.Database.Repository;

namespace ApiIdempotent.Infrastructure.Services.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly ILogger<ContaCorrenteController> _logger;

        public ContaCorrenteController(ILogger<ContaCorrenteController> logger)
        {
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("{idContaCorrente}/saldo")]
        [HttpGet()]
        public ActionResult<ConsultaSaldoResponse> Get(
            [FromServices] SaldoRepository saldoRepository,
            [FromServices] ContaCorrenteRepository contaCorrenteRepository,
            string idContaCorrente)
        {
            if(!contaCorrenteRepository.IsValidAccount(idContaCorrente))
                return NotFound("Conta inexistente.");

            if(!contaCorrenteRepository.IsActiveAccount(idContaCorrente))   
                return BadRequest("Conta inativa para esta operação.");

            return Ok(saldoRepository.GetSaldo(idContaCorrente));
        }


        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("{idContaCorrente}/movimentacao")]
        [Idempotent(Enabled = true, ExpireHours = 24)]
        [HttpPost()]
        public IActionResult Post(
            [FromServices] IUnitOfWork unitOfWork,
            [FromServices] MovimentoRepository movimentoRepository,
            [FromServices] ContaCorrenteRepository contaCorrenteRepository,
            [FromRoute] string idContaCorrente,
            CriarMovimentacaoRequest request)
        {
            if (!contaCorrenteRepository.IsValidAccount(idContaCorrente))
                return NotFound("Conta inexistente.");

            if (!contaCorrenteRepository.IsActiveAccount(idContaCorrente))
                return BadRequest("Conta inativa para esta operação.");

            if (request.Valor is null || request.Valor <= 0)
                return BadRequest("Valor inválido para esta operação.");

            if (request.TipoMovimento is null || (!request.TipoMovimento.Equals("D") && !request.TipoMovimento.Equals("C")) )
                return BadRequest("Tipo de Movimento inválido para esta operação.");

            if (!contaCorrenteRepository.IsActiveAccount(idContaCorrente))
                return BadRequest("Conta inativa para esta operação.");

            if (request is null)
                return BadRequest("Requisição vazia.");


            unitOfWork.BeginTransaction();

            movimentoRepository.Save(idContaCorrente, request);

            unitOfWork.Commit();

            return Ok();
        }
          
    }
}