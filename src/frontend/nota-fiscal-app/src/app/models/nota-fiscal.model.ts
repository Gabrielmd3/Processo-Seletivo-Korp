import { NotaFiscalItem } from "./nota-fiscal-item.model";

export type NotaStatus = 'Aberta' | 'Processando' | 'Fechada' | 'Cancelada';

export interface NotaFiscal {
  id: string;
  numero: number;
  status: NotaStatus;
  dataEmissao: string;
  itens: NotaFiscalItem[];
}