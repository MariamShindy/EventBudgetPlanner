import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { EventService } from '../../../core/services/event.service';
import { Event, EventFilter } from '../../../core/models/event.models';

@Component({
  selector: 'app-event-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, DatePipe, CurrencyPipe],
  templateUrl: './event-list.html',
  styleUrl: './event-list.scss'
})
export class EventListComponent implements OnInit {
  private eventService = inject(EventService);
  private fb = inject(FormBuilder);
  private router = inject(Router);

  events: Event[] = [];
  isLoading = false;
  filterForm: FormGroup;
  errorMessage = '';
  constructor() {
    this.filterForm = this.fb.group({
      searchTerm: [''],
      startDate: [''],
      endDate: [''],
      minBudget: [''],
      maxBudget: ['']
    });
  }

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
  this.isLoading = true;
  this.errorMessage = '';

  // Get form values and build filter
  const formValue = this.filterForm.value;
  const filter: EventFilter = {
    searchTerm: formValue.searchTerm || undefined,
    startDate: formValue.startDate || undefined,
    endDate: formValue.endDate || undefined,
    minBudget: formValue.minBudget ? Number(formValue.minBudget) : undefined,
    maxBudget: formValue.maxBudget ? Number(formValue.maxBudget) : undefined,
    page: 1,
    pageSize: 50
  };

  this.eventService.filter(filter).subscribe({
    next: (data) => {
      this.events = data;
      this.isLoading = false;
    },
    error: (error) => {
      this.errorMessage = error.error?.message || 'Failed to load events.';
      this.isLoading = false;
    }
  });
}

  resetFilters(): void {
    this.filterForm.reset();
    this.loadEvents();
  }

  deleteEvent(eventId: number): void {
  if (!confirm('Are you sure you want to delete this event?')) {
    return;
  }

  this.isLoading = true;
  this.errorMessage = '';

  this.eventService.delete(eventId).subscribe({
    next: () => this.loadEvents(),
    error: err => {
      this.errorMessage = err.error?.message || 'Failed to delete event.';
      this.isLoading = false;
    }
  });
 }

  openCreate(): void {
    this.router.navigate(['/events/create']);
  }

  openDetail(eventId: number): void {
    this.router.navigate(['/events', eventId]);
  }

  editEvent(eventId: number): void {
    this.router.navigate(['/events', eventId, 'edit']);
  }
}