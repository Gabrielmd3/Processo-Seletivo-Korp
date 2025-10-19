import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceModal } from './invoice-modal';

describe('InvoiceModal', () => {
  let component: InvoiceModal;
  let fixture: ComponentFixture<InvoiceModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InvoiceModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InvoiceModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
