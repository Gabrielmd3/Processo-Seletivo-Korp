using FaturamentoService.Models;

namespace FaturamentoService.Dtos
{
    public class NotaFiscalResponseDto
    {
        public Guid Id { get; set; }
        public int Numero { get; set; }
        public NotaStatus Status { get; set; }
        public DateTime DataEmissao { get; set; }
        public List<NotaFiscalItemResponseDto> Itens { get; set; } = new();
    }
}