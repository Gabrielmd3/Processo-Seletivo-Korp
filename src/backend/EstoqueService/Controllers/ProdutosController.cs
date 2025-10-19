using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstoqueService.Data;
using EstoqueService.Models;
using EstoqueService.Dtos;

namespace EstoqueService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        // 1. Injetando o DbContext específico
        private readonly EstoqueDbContext _context;

        public ProdutosController(EstoqueDbContext context)
        {
            _context = context;
        }

        // GET: /api/produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            // 2. Usando a classe Produto diretamente
            var produtos = await _context.Produtos.AsNoTracking().ToListAsync();
            return Ok(produtos);
        }

        // GET: /api/produtos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(Guid id)
        {
            // 3. Usando Guid como tipo do ID.
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            return Ok(produto);
        }

        // POST: /api/produtos
        [HttpPost]
        public async Task<ActionResult<Produto>> CreateProduto(CreateProdutoDto produtoDto)
        {
            // 4. Usando um DTO para a criação.
            var produto = new Produto
            {
                Id = Guid.NewGuid(),
                Nome = produtoDto.Nome,
                Preco = produtoDto.Preco,
                SaldoEstoque = produtoDto.SaldoEstoque
            };

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            // Retorno HTTP 201 Created
            return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);
        }

        // PUT: /api/produtos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduto(Guid id, UpdateProdutoDto produtoDto)
        {
            var produtoNoDb = await _context.Produtos.FindAsync(id);

            if (produtoNoDb == null)
            {
                return NotFound();
            }

            // 5. busca a entidade e atualiza as propriedades.
            produtoNoDb.Nome = produtoDto.Nome;
            produtoNoDb.Preco = produtoDto.Preco;
            produtoNoDb.SaldoEstoque = produtoDto.SaldoEstoque;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Adicionar tratamento para concorrência se necessário
                throw;
            }

            return NoContent(); // Retorno HTTP 204 No Content
        }

        // DELETE: /api/produtos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(Guid id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: /api/produtos/{id}/dar-baixa
        [HttpPut("{id}/dar-baixa")]
        public async Task<IActionResult> DarBaixaEstoque(Guid id, [FromBody] int quantidade)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound(new { Mensagem = "Produto não encontrado." });
            }

            if (produto.SaldoEstoque < quantidade)
            {
                // Retorno HTTP 409 Conflict.
                return Conflict(new { Mensagem = $"Estoque insuficiente para o produto {produto.Nome}." });
            }

            produto.SaldoEstoque -= quantidade;
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = "Baixa no estoque realizada com sucesso." });
        }

        // PUT: /api/produtos/{id}/compensar-baixa
        [HttpPut("{id}/compensar-baixa")]
        public async Task<IActionResult> CompensarBaixaEstoque(Guid id, [FromBody] int quantidade)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return Ok(new { Mensagem = "Produto não encontrado durante a compensação, mas o processo continuará." });
            }

            produto.SaldoEstoque += quantidade; // Devolve a quantidade ao estoque
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = "Estoque compensado com sucesso." });
        }

        // POST: /api/produtos/details
        [HttpPost("details")]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutosDetails([FromBody] IEnumerable<Guid> produtoIds)
        {
            if (produtoIds == null || !produtoIds.Any())
            {
                return BadRequest("É necessário fornecer uma lista de IDs de produtos.");
            }
            var produtos = await _context.Produtos
                                         .Where(p => produtoIds.Contains(p.Id))
                                         .ToListAsync();

            return Ok(produtos);
        }
    }
}