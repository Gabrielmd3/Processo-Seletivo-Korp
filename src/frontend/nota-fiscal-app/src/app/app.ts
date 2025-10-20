import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Navbar } from "./components/navbar/navbar";
import { ToastComponent } from './components/toast/toast';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Navbar, ToastComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('nota-fiscal-app');
}
