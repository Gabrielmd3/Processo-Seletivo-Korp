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
        public async Task<ActionResult<IEnumerable<NotaFiscal>>> GetNotasFiscais([FromQuery] NotaStatus? status = null)
        {
            // 1. Começamos com uma consulta base (IQueryable) que ainda não foi executada.
            var query = _context.NotasFiscais
                                .Include(nf => nf.Itens)
                                .AsQueryable();

            // 2. Se um filtro de status foi fornecido na URL, adicionamos uma cláusula WHERE.
            if (status.HasValue)
            {
                query = query.Where(nf => nf.Status == status.Value);
            }

            // 3. Adicionamos a ordenação (sempre as mais recentes primeiro) e executamos a consulta.
            var notas = await query.OrderByDescending(nf => nf.DataEmissao)
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

                if (itemDto.Quantidade <= 0)
                {
                    return Conflict(new { Mensagem = $"Selecione uma quantidade minima para o produto '{produtoDoEstoque.Nome}'" });
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

        // POST: api/notasfiscais/{id}/imprimir
        [HttpPost("{id}/imprimir")]
        public async Task<IActionResult> ImprimirNotaFiscal(Guid id)
        {
            var notaFiscal = await _context.NotasFiscais
                                           .Include(nf => nf.Itens)
                                           .FirstOrDefaultAsync(nf => nf.Id == id);

            if (notaFiscal == null)
            {
                return NotFound(new { Mensagem = "Nota fiscal não encontrada." });
            }

            if (notaFiscal.Status != NotaStatus.Aberta)
            {
                return Conflict(new { Mensagem = $"Não é possível processar uma nota com status '{notaFiscal.Status}'." });
            }

            // Bloqueia a nota para evitar processamento duplo
            notaFiscal.Status = NotaStatus.Processando;
            await _context.SaveChangesAsync();

            var httpClient = _httpClientFactory.CreateClient("EstoqueService");
            // Lista para rastrear os itens que tiveram baixa com sucesso
            var itensComBaixaSucesso = new List<NotaFiscalItem>();

            // --- PASSO 2: Execução da Saga ---
            foreach (var item in notaFiscal.Itens)
            {
                // Prepara o conteúdo da requisição
                var jsonContent = new StringContent(item.Quantidade.ToString(), System.Text.Encoding.UTF8, "application/json");

                try
                {
                    // Tenta dar baixa no estoque
                    var response = await httpClient.PutAsync($"/api/produtos/{item.ProdutoId}/dar-baixa", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        // Se deu certo, adiciona o item à lista de sucesso
                        itensComBaixaSucesso.Add(item);
                    }
                    else
                    {
                        // Se falhou, inicia a compensação
                        var errorContent = await response.Content.ReadAsStringAsync();
                        await CompensarTransacao(httpClient, itensComBaixaSucesso);

                        notaFiscal.Status = NotaStatus.Cancelada;
                        await _context.SaveChangesAsync();

                        return Conflict(new { Mensagem = "Falha ao dar baixa no estoque. A transação foi revertida.", Causa = errorContent });
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Se o serviço de estoque estiver fora do ar
                    await CompensarTransacao(httpClient, itensComBaixaSucesso);

                    notaFiscal.Status = NotaStatus.Cancelada;
                    await _context.SaveChangesAsync();

                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new { Mensagem = "O serviço de estoque está indisponível. A transação foi revertida.", Detalhes = ex.Message });
                }
            }

            // --- PASSO 3: Finalização ---
            notaFiscal.Status = NotaStatus.Fechada;
            await _context.SaveChangesAsync();

            // Retorna a nota fiscal atualizada
            httpClient = _httpClientFactory.CreateClient("EstoqueService");
            var produtoIds = notaFiscal.Itens.Select(item => item.ProdutoId).ToList();
            var responseDetails = await httpClient.PostAsJsonAsync("/api/produtos/details", produtoIds);
            var produtosDetails = new List<ProdutoDto>();

            // Se a chamada foi bem-sucedida, lê os detalhes dos produtos
            if (responseDetails.IsSuccessStatusCode)
            {
                produtosDetails = await responseDetails.Content.ReadFromJsonAsync<List<ProdutoDto>>() ?? new List<ProdutoDto>();
            }

            // Mapeia para o DTO de resposta incluindo os nomes dos produtos
            var responseDto = new NotaFiscalResponseDto
            {
                Id = notaFiscal.Id,
                Numero = notaFiscal.Numero,
                Status = notaFiscal.Status,
                DataEmissao = notaFiscal.DataEmissao,
                Itens = notaFiscal.Itens.Select(item => new NotaFiscalItemResponseDto
                {
                    Id = item.Id,
                    ProdutoId = item.ProdutoId,
                    ProdutoNome = produtosDetails?.FirstOrDefault(p => p.Id == item.ProdutoId)?.Nome ?? "Produto não encontrado",
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario
                }).ToList()
            };
            return Ok(responseDto);
        }

        // Método auxiliar privado para a lógica de compensação
        private async Task CompensarTransacao(HttpClient httpClient, List<NotaFiscalItem> itensParaCompensar)
        {
            // Passa por cada item que já teve baixa e devolve a quantidade ao estoque
            foreach (var item in itensParaCompensar)
            {
                var jsonContent = new StringContent(item.Quantidade.ToString(), System.Text.Encoding.UTF8, "application/json");
                await httpClient.PutAsync($"/api/produtos/{item.ProdutoId}/compensar-baixa", jsonContent);
            }
        }
    }
}