import { Routes } from '@angular/router';
import { ProdutoList } from './pages/produto-list/produto-list';
import { NotaFiscalList } from './pages/nota-fiscal-list/nota-fiscal-list';


export const routes: Routes = [
  { path: '', redirectTo: '/produtos', pathMatch: 'full' },
  { path: 'produtos', component: ProdutoList },
  { path: 'notas-fiscais', component: NotaFiscalList },

];
