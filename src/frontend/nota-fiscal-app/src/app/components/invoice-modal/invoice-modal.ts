import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotaFiscal } from '../../models/nota-fiscal.model';

@Component({
  selector: 'app-invoice-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './invoice-modal.html',
  styleUrl: './invoice-modal.scss'
})
export class InvoiceModalComponent {
  // @Input() permite que o componente pai passe dados para este componente
  @Input() nota: NotaFiscal | null = null;

  // @Output() permite que o componente emita eventos para o componente pai
  @Output() close = new EventEmitter<void>();

  // Função para fechar o modal
  onClose(): void {
    this.close.emit();
  }

  // Função para acionar a impressão do navegador
  onPrint(): void {
    window.print();
  }

  // Função auxiliar para calcular o total
  getTotal(): number {
    if (!this.nota) return 0;
    return this.nota.itens.reduce((total, item) => total + (item.quantidade * item.precoUnitario), 0);
  }
}