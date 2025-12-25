import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
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
  private location = inject(Location);

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
    // Check for template data from navigation state (works during navigation)
    const navigation = this.router.getCurrentNavigation();
    let templateData = navigation?.extras?.state?.['template'];
    
    // If not found in navigation, check history state using Location service
    if (!templateData) {
      const state = this.location.getState() as any;
      if (state && state.template) {
        templateData = state.template;
      }
    }
    
    // If still not found, check window history as fallback
    if (!templateData && (window.history.state && window.history.state.template)) {
      templateData = window.history.state.template;
    }
    
    if (templateData) {
      this.form.patchValue({
        name: templateData.name || '',
        date: templateData.date || '',
        budget: templateData.budget || 0,
        description: templateData.description || '',
        currencyCode: templateData.currencyCode || 'USD',
        eventTemplateId: templateData.eventTemplateId || null
      });
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