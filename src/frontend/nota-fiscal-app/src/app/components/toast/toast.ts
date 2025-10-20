import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService } from '../../services/toast';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './toast.html',
  styleUrls: ['./toast.scss']
})
export class ToastComponent {
  constructor(public toastService: ToastService) {}

  // Determina a cor de fundo com base no tipo de toast (erro, sucesso, etc.)
  getBackgroundColorClass() {
    const type = this.toastService.state().type;
    switch (type) {
      case 'error':
        return 'bg-red-500';
      case 'success':
        return 'bg-green-500';
      default:
        return 'bg-blue-500';
    }
  }
}