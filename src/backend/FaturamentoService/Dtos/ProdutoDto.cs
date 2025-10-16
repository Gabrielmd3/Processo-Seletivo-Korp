namespace FaturamentoService.Dtos
{
    public record ProdutoDto(Guid Id, string Nome, decimal Preco, int SaldoEstoque);
}