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
    }
}