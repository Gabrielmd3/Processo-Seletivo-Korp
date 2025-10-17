using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FaturamentoService.Data;
using FaturamentoService.Models;
using FaturamentoService.Dtos;

namespace FaturamentoService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotasFiscaisController : ControllerBase
    {
        private readonly FaturamentoDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public NotasFiscaisController(FaturamentoDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // GET: api/notasfiscais
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotaFiscal>>> GetNotasFiscais()
        {
            var notas = await _context.NotasFiscais
                                      .Include(nf => nf.Itens)
                                      .AsNoTracking()
                                      .ToListAsync();
            return Ok(notas);
        }

        // GET: api/notasfiscais/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<NotaFiscal>> GetNotaFiscal(Guid id)
        {
            var notaFiscal = await _context.NotasFiscais
                                           .Include(nf => nf.Itens)
                                           .FirstOrDefaultAsync(nf => nf.Id == id);

            if (notaFiscal == null)
            {
                return NotFound();
            }

            return Ok(notaFiscal);
        }

        // POST: api/notasfiscais
        [HttpPost]
        public async Task<ActionResult<NotaFiscal>> CreateNotaFiscal(CreateNotaFiscalDto notaFiscalDto)
        {
            // Validação inicial
            if (notaFiscalDto.Itens == null || !notaFiscalDto.Itens.Any())
            {
                return BadRequest(new { Mensagem = "A nota fiscal deve conter pelo menos um item." });
            }

            // 1. Criar um cliente HTTP para se comunicar com o EstoqueService
            var httpClient = _httpClientFactory.CreateClient("EstoqueService");
            var itensParaNota = new List<NotaFiscalItem>();

            // 2. Iterar sobre os itens enviados pelo cliente para validar e buscar os preços
            foreach (var itemDto in notaFiscalDto.Itens)
            {
                ProdutoDto? produtoDoEstoque;
                try
                {
                    // Faz a chamada GET para /api/produtos/{produtoId} no EstoqueService
                    produtoDoEstoque = await httpClient.GetFromJsonAsync<ProdutoDto>($"/api/produtos/{itemDto.ProdutoId}");
                }
                catch (HttpRequestException ex)
                {
                    // Se o EstoqueService estiver fora do ar ou retornar um erro inesperado
                    return StatusCode(StatusCodes.Status503ServiceUnavailable,
                        new { Mensagem = "O serviço de estoque está temporariamente indisponível. Tente novamente mais tarde.", Detalhes = ex.Message });
                }

                // Valida se o produto existe no estoque
                if (produtoDoEstoque == null)
                {
                    return NotFound(new { Mensagem = $"Produto com ID {itemDto.ProdutoId} não foi encontrado." });
                }

                // Valida se há estoque suficiente já na criação
                if (produtoDoEstoque.SaldoEstoque < itemDto.Quantidade)
                {
                    return Conflict(new { Mensagem = $"Estoque insuficiente para o produto '{produtoDoEstoque.Nome}'. Saldo atual: {produtoDoEstoque.SaldoEstoque}." });
                }

                // 3. Cria o item da nota fiscal
                var novoItem = new NotaFiscalItem
                {
                    Id = Guid.NewGuid(),
                    ProdutoId = itemDto.ProdutoId,
                    Quantidade = itemDto.Quantidade,
                    PrecoUnitario = produtoDoEstoque.Preco
                };
                itensParaNota.Add(novoItem);
            }

            // 4. Se todas as validações passaram, cria e salva a nota fiscal no banco
            var notaFiscal = new NotaFiscal
            {
                Id = Guid.NewGuid(),
                Status = NotaStatus.Aberta,
                DataEmissao = DateTime.UtcNow,
                Numero = new Random().Next(1000, 9999),
                Itens = itensParaNota // Adiciona a lista de itens já validados
            };

            _context.NotasFiscais.Add(notaFiscal);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNotaFiscal), new { id = notaFiscal.Id }, notaFiscal);
        }

    }
}