import { Injectable, signal } from '@angular/core';

export type ToastType = 'success' | 'error' | 'info';

export interface ToastState {
  message: string;
  type: ToastType;
  show: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  public state = signal<ToastState>({ message: '', type: 'info', show: false });

  show(message: string, type: ToastType = 'info') {
    this.state.set({ message, type, show: true });
    setTimeout(() => this.hide(), 5000); // O toast desaparecerá após 5 segundos
  }

  hide() {
    this.state.set({ ...this.state(), show: false });
  }
}