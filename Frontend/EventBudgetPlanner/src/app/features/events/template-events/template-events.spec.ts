import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TemplateEventsComponent } from './template-events';

describe('TemplateEventsComponent', () => {
  let component: TemplateEventsComponent;
  let fixture: ComponentFixture<TemplateEventsComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TemplateEventsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TemplateEventsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
