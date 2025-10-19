import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProdutoService, UpdateProdutoDto } from '../../services/produto';
import { Produto } from '../../models/produto.model';

// Componente para listar, criar, editar e deletar produtos
@Component({
  selector: 'app-produto-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './produto-list.html',
  styleUrl: './produto-list.scss'
})

// Classe do componente
export class ProdutoList implements OnInit {
  produtos: Produto[] = [];
  isLoading = true;
  errorMessage = '';
  showForm = false;
  produtoForm: FormGroup;
  editingProductId: string | null = null;
  editForm: FormGroup;

  constructor(private produtoService: ProdutoService, private fb: FormBuilder, private cdr: ChangeDetectorRef) {
    // Inicializa o formulário com os campos necessários
    this.produtoForm = this.fb.group({
      nome: ['', Validators.required],
      preco: [0, [Validators.required, Validators.min(0.01)]],
      saldoEstoque: [0, [Validators.required, Validators.min(0)]]
    });

    this.editForm = this.fb.group({
      preco: [0, [Validators.required, Validators.min(0.01)]],
      saldoEstoque: [0, [Validators.required, Validators.min(0)]]
    });
  }

  // Carrega os produtos ao iniciar o componente
  ngOnInit(): void {
    this.loadProdutos();
  }

  // Carrega a lista de produtos do backend
  loadProdutos(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.produtoService.getProdutos().subscribe({
      next: (data) => {
        this.produtos = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.errorMessage = 'Falha ao carregar os produtos. Verifique se o backend está rodando.';
        console.error(err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  onSubmit(): void {
    if (this.produtoForm.invalid) {
      return;
    }

    // Chama o serviço para criar o produto
    this.produtoService.createProduto(this.produtoForm.value).subscribe({
      next: () => {
        this.produtoForm.reset();
        this.showForm = false;
        this.loadProdutos();
      },
      error: (err) => {
        this.errorMessage = 'Falha ao criar o produto.';
        console.error(err);
      }
    });
  }

  // Inicia a edição de um produto
  onEdit(produto: Produto): void {
    this.editingProductId = produto.id;
    this.editForm.patchValue({
      preco: produto.preco,
      saldoEstoque: produto.saldoEstoque
    });
  }

  // Cancela a edição
  onCancelEdit(): void {
    this.editingProductId = null;
  }

  // Salva as alterações feitas no produto
  onSaveEdit(produtoParaAtualizar: Produto): void {
    if (this.editForm.invalid) {
      return;
    }

    // Monta o DTO com o nome original e os valores atualizados do formulário
    const updateDto: UpdateProdutoDto = {
      nome: produtoParaAtualizar.nome,
      preco: this.editForm.value.preco,
      saldoEstoque: this.editForm.value.saldoEstoque
    };

    this.produtoService.updateProduto(produtoParaAtualizar.id, updateDto).subscribe({
      next: () => {
        const index = this.produtos.findIndex(p => p.id === produtoParaAtualizar.id);
        if (index !== -1) {
          this.produtos[index].preco = updateDto.preco;
          this.produtos[index].saldoEstoque = updateDto.saldoEstoque;
        }
        this.editingProductId = null;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.errorMessage = 'Falha ao atualizar o produto.';
        console.error(err);
      }
    });
  }

  // Deleta um produto pelo ID
  onDelete(id: string): void {
    // Pede confirmação antes de excluir
    if (confirm('Tem certeza que deseja excluir este produto?')) {
      this.produtoService.deleteProduto(id).subscribe({
        next: () => {
          this.loadProdutos();
        },
        error: (err) => {
          this.errorMessage = 'Falha ao excluir o produto.';
          console.error(err);
        }
      });
    }
  }

}
