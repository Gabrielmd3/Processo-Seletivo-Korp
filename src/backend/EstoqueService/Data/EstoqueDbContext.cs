using EstoqueService.Models; // Para ter acesso à classe 'Produto'
using Microsoft.EntityFrameworkCore;

namespace EstoqueService.Data
{
    // A classe herda de DbContext, que é a classe base do Entity Framework
    public class EstoqueDbContext : DbContext
    {
        // Este construtor é necessário para a injeção de dependência, que veremos no Passo 4
        public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options)
        {
        }

        // Esta propriedade representa a tabela "Produtos" no seu banco de dados.
        // O EF Core irá criar uma tabela chamada "Produtos" com colunas baseadas
        // nas propriedades da sua classe 'Produto'.
        public DbSet<Produto> Produtos { get; set; }
    }
}