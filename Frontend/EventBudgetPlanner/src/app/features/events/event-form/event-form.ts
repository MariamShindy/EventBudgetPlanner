import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { EventService } from '../../../core/services/event.service';
import { CreateEventRequest, Event } from '../../../core/models/event.models';

@Component({
  selector: 'app-event-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './event-form.html',
  styleUrl: './event-form.scss'
})
export class EventFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private eventService = inject(EventService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  form: FormGroup;
  isEditMode = false;
  eventId: number | null = null;
  isLoading = false;
  errorMessage = '';

  constructor() {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      date: ['', Validators.required],
      budget: [0, [Validators.required, Validators.min(1)]],
      description: [''],
      currencyCode: ['USD'],
      eventTemplateId: [null]
    });
  }

  ngOnInit(): void {
    const navigation = this.router.getCurrentNavigation();
    const templateData = navigation?.extras?.state?.['template'];
    if (templateData) {
      this.form.patchValue(templateData);
    }

    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.isEditMode = true;
        this.eventId = +id;
        this.loadEvent(+id);
      }
    });
  }

  loadEvent(id: number): void {
    this.isLoading = true;
    this.eventService.getById(id).subscribe({
      next: (event) => {
        this.form.patchValue({
          name: event.name,
          date: event.date.split('T')[0],
          budget: event.budget,
          description: event.description,
          currencyCode: 'USD'
        });
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to load event.';
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    const payload: CreateEventRequest = this.form.value;

    if (this.isEditMode && this.eventId) {
      this.eventService.update(this.eventId, payload).subscribe({
        next: () => {
          this.router.navigate(['/events', this.eventId]);
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Failed to update event.';
          this.isLoading = false;
        }
      });
    } else {
      this.eventService.create(payload).subscribe({
        next: (created) => {
          this.router.navigate(['/events', created.id]);
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Failed to create event.';
          this.isLoading = false;
        }
      });
    }
  }
}