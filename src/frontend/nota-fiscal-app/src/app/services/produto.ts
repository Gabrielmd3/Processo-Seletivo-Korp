// src/app/services/produto.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Produto } from '../models/produto.model';

export interface UpdateProdutoDto {
  nome: string;
  preco: number;
  saldoEstoque: number;
}

@Injectable({
  providedIn: 'root'
})
export class ProdutoService {
  // A URL base da API de Estoque.
  private apiUrl = '/api/produtos';

  constructor(private http: HttpClient) { }

  // Busca a lista completa de produtos do backend.
  getProdutos(): Observable<Produto[]> {
    return this.http.get<Produto[]>(this.apiUrl);
  }


  // Busca um único produto pelo seu ID.
  getProdutoPorId(id: string): Observable<Produto> {
    return this.http.get<Produto>(`${this.apiUrl}/${id}`);
  }

  // Cria um novo produto no backend.
  createProduto(produto: Omit<Produto, 'id'>): Observable<Produto> {
    return this.http.post<Produto>(this.apiUrl, produto);
  }

  // Atualiza um produto existente pelo seu ID.
  updateProduto(id: string, produto: UpdateProdutoDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, produto);
  }

  // Deleta um produto pelo seu ID.
  deleteProduto(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}