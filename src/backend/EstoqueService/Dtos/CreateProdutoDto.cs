namespace EstoqueService.Dtos
{
    // DTO para criar um produto. Não inclui o Id, pois ele será gerado pelo servidor.
    public record CreateProdutoDto(string Nome, decimal Preco, int SaldoEstoque);
}