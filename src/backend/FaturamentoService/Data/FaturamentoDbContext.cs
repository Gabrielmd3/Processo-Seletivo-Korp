using FaturamentoService.Models; // Para ter acesso à classe 'Produto'
using Microsoft.EntityFrameworkCore;

namespace FaturamentoService.Data
{
    // A classe herda de DbContext, que é a classe base do Entity Framework
    public class FaturamentoDbContext : DbContext
    {
        // Este construtor é necessário para a injeção de dependência, que veremos no Passo 4
        public FaturamentoDbContext(DbContextOptions<FaturamentoDbContext> options) : base(options)
        {
        }

        // Esta propriedade representa a tabela "NotasFiscais" no seu banco de dados.
        // O EF Core irá criar uma tabela chamada "NotasFiscais" com colunas baseadas
        // nas propriedades da sua classe 'NotaFiscal'.
        public DbSet<NotaFiscal> NotasFiscais { get; set; }

        // Esta propriedade representa a tabela "NotaFiscalItens" no seu banco de dados.
        // O EF Core irá criar uma tabela chamada "NotaFiscalItens" com colunas baseadas
        // nas propriedades da sua classe 'NotaFiscalItem'.
        public DbSet<NotaFiscalItem> NotaFiscalItens { get; set; }
    }
}