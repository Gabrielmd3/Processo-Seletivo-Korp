import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { NotaFiscalService } from '../../services/nota-fiscal';
import { NotaFiscal, NotaStatus } from '../../models/nota-fiscal.model';

// Componente para listar notas fiscais
@Component({
  selector: 'app-nota-fiscal-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './nota-fiscal-list.html',
  styleUrl: './nota-fiscal-list.scss'
})
export class NotaFiscalList implements OnInit {
  notasFiscais: NotaFiscal[] = [];
  isLoading = true;
  errorMessage = '';

  activeFilter: NotaStatus | 'Todos' = 'Todos';

  constructor(private notaFiscalService: NotaFiscalService, private cdr: ChangeDetectorRef) { }

  // Carrega as notas fiscais ao iniciar o componente
  ngOnInit(): void {
    this.loadNotasFiscais();
  }

  // Carrega a lista de notas fiscais do backend com base no filtro ativo
  loadNotasFiscais(): void {
    this.isLoading = true;
    this.errorMessage = '';
    const filterToSend = this.activeFilter === 'Todos' ? undefined : this.activeFilter;

    this.notaFiscalService.getNotasFiscais(filterToSend).subscribe({
      next: (data) => {
        this.notasFiscais = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.errorMessage = 'Falha ao carregar as notas fiscais. Verifique se o backend de faturamento está rodando.';
        console.error(err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  // Manipula a mudança de filtro pelo usuário
  onFilterChange(filter: NotaStatus | 'Todos'): void {
    this.activeFilter = filter;
    this.loadNotasFiscais();
  }

  // Manipula a ação de imprimir uma nota fiscal
  onImprimir(id: string): void {
    this.notaFiscalService.imprimirNotaFiscal(id).subscribe({
      next: () => {
        const index = this.notasFiscais.findIndex(nf => nf.id === id);
        if (index !== -1) {
          this.loadNotasFiscais();
        }
      },
      error: (err) => {
        // Exibe a mensagem de erro específica vinda do backend
        this.errorMessage = err.error?.mensagem || 'Ocorreu um erro ao processar a nota.';
        console.error(err);
        this.loadNotasFiscais();
      }
    });
  }

  // Calcula o total da nota fiscal somando os itens
  getTotalNota(nota: NotaFiscal): number {
    return nota.itens.reduce((total, item) => total + (item.quantidade * item.precoUnitario), 0);
  }

  // Retorna a classe CSS baseada no status da nota fiscal
  getStatusClass(status: NotaStatus): string {
    switch (status) {
      case 'Aberta': return 'bg-blue-100 text-blue-800';
      case 'Processando': return 'bg-yellow-100 text-yellow-800';
      case 'Fechada': return 'bg-green-100 text-green-800';
      case 'Cancelada': return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }
}