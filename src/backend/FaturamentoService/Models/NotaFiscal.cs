namespace FaturamentoService.Models
{
    public enum NotaStatus { Aberta, Fechada, Cancelada, Processando }
    public class NotaFiscal
    {
        public Guid Id { get; set; }
        public int Numero { get; set; }
        public NotaStatus Status { get; set; }
        public DateTime DataEmissao { get; set; }
        public List<NotaFiscalItem> Itens { get; set; } = new();
    }
}
