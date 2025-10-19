import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Produto } from '../../models/produto.model';
import { ProdutoService } from '../../services/produto';
import { NotaFiscalService, CreateNotaDto } from '../../services/nota-fiscal';

// Interface auxiliar para facilitar o gerenciamento do "carrinho"
interface CartItem {
  product: Produto;
  quantity: number;
}

// Componente para criação de nota fiscal
@Component({
  selector: 'app-nota-fiscal-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './nota-fiscal-create.html',
  styleUrl: './nota-fiscal-create.scss'
})
export class NotaFiscalCreate implements OnInit {
  allProducts: Produto[] = [];
  cartItems: CartItem[] = [];

  addItemForm: FormGroup;

  isLoading = true;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private produtoService: ProdutoService,
    private notaFiscalService: NotaFiscalService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    // Formulário para adicionar um item à nota
    this.addItemForm = this.fb.group({
      productId: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]]
    });
  }

  // Carrega a lista de produtos ao iniciar o componente
  ngOnInit(): void {
    this.loadAllProducts();
  }

  // Carrega todos os produtos disponíveis
  loadAllProducts(): void {
    this.isLoading = true;
    this.produtoService.getProdutos().subscribe({
      next: (products) => {
        this.allProducts = products;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Falha ao carregar a lista de produtos. Verifique o serviço de estoque.';
        this.isLoading = false;
        this.cdr.detectChanges();
        
      }
    });
  }

  // Adiciona um item ao "carrinho"
  onAddItem(): void {
    if (this.addItemForm.invalid) {
      return;
    }

    const { productId, quantity } = this.addItemForm.value;
    const selectedProduct = this.allProducts.find(p => p.id === productId);

    if (selectedProduct) {
      this.cartItems.push({ product: selectedProduct, quantity: quantity });
      this.addItemForm.reset({ quantity: 1 });
    }
  }

  // Remove um item do "carrinho"
  onRemoveItem(productId: string): void {
    this.cartItems = this.cartItems.filter(item => item.product.id !== productId);
  }

  // Calcula o total da nota fiscal
  getTotal(): number {
    return this.cartItems.reduce((total, item) => total + (item.product.preco * item.quantity), 0);
  }

  // Cria a nota fiscal
  onCreateNota(): void {
    if (this.cartItems.length === 0) {
      this.errorMessage = 'Adicione pelo menos um item à nota fiscal.';
      return;
    }

    // Transforma nosso "carrinho" no DTO que a API espera
    const dto: CreateNotaDto = {
      itens: this.cartItems.map(item => ({
        produtoId: item.product.id,
        quantidade: item.quantity
      }))
    };

    this.notaFiscalService.createNotaFiscal(dto).subscribe({
      next: () => {
        this.router.navigate(['/notas-fiscais']);
      },
      error: (err) => {
        this.errorMessage = err.error?.mensagem || 'Ocorreu um erro ao criar a nota fiscal.';
      }
    });
  }
}