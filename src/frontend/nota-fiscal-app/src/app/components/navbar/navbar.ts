import { Component } from '@angular/core';
// 1. Importe as diretivas de roteamento aqui
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  // 2. Adicione as diretivas ao array de imports
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class Navbar {

}