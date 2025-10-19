import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NotaFiscalList } from './nota-fiscal-list';

// describe a suite de testes para o componente NotaFiscalList
describe('NotaFiscalList', () => {
  let component: NotaFiscalList;
  let fixture: ComponentFixture<NotaFiscalList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotaFiscalList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NotaFiscalList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
