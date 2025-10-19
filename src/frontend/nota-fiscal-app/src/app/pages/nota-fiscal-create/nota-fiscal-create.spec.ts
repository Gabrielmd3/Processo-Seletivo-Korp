import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NotaFiscalCreate } from './nota-fiscal-create';

// Teste básico do componente NotaFiscalCreate
describe('NotaFiscalCreate', () => {
  let component: NotaFiscalCreate;
  let fixture: ComponentFixture<NotaFiscalCreate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotaFiscalCreate]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NotaFiscalCreate);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
