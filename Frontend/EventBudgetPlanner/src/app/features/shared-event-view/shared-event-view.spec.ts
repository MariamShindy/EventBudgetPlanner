import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SharedEventView } from '.';

describe('SharedEventView', () => {
  let component: SharedEventView;
  let fixture: ComponentFixture<SharedEventView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SharedEventView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SharedEventView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
