import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { NotaFiscal, NotaStatus } from '../models/nota-fiscal.model';

// Este é o "contrato" para criar uma nova nota.
// O frontend só precisa enviar uma lista de itens com o ID do produto e a quantidade.
export interface CreateNotaDto {
  itens: {
    produtoId: string;
    quantidade: number;
  }[];
}

@Injectable({
  providedIn: 'root'
})
export class NotaFiscalService {
  // A URL base para os endpoints de Notas Fiscais.
  private apiUrl = '/api/notasfiscais';

  constructor(private http: HttpClient) { }

  // Busca a lista de notas fiscais, opcionalmente filtrando por status.
  getNotasFiscais(status?: NotaStatus): Observable<NotaFiscal[]> {
    let params = new HttpParams();
    if (status) {
      params = params.set('status', status);
    }
    return this.http.get<NotaFiscal[]>(this.apiUrl, { params });
  }

  // Cria uma nova nota fiscal com os dados fornecidos.
  createNotaFiscal(dto: CreateNotaDto): Observable<NotaFiscal> {
    return this.http.post<NotaFiscal>(this.apiUrl, dto);
  }

  // Imprime (processa) uma nota fiscal pelo seu ID.
  imprimirNotaFiscal(id: string): Observable<NotaFiscal> {
    return this.http.post<NotaFiscal>(`${this.apiUrl}/${id}/imprimir`, {});
  }
}