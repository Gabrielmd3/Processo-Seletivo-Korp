namespace EstoqueService.Models // <-- ESTA É A LINHA MAIS IMPORTANTE!
{
    public class Produto
    {
        public Guid Id { get; set; }
        public required string Nome { get; set; }
        public required decimal Preco { get; set; }
        public int SaldoEstoque { get; set; }
    }
}