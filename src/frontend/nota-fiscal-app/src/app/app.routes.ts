import { Routes } from '@angular/router';
import { ProdutoList } from './pages/produto-list/produto-list';

export const routes: Routes = [
  { path: '', redirectTo: '/produtos', pathMatch: 'full' },
  { path: 'produtos', component: ProdutoList },
];
