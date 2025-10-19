export interface NotaFiscalItem {
  id: string;
  notaFiscalId: string;
  produtoId: string;
  produtoNome?: string;
  quantidade: number;
  precoUnitario: number;
}