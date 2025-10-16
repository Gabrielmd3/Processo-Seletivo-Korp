namespace EstoqueService.Dtos
{
    // DTO para atualizar um produto.
    public record UpdateProdutoDto(string Nome, decimal Preco, int SaldoEstoque);
}