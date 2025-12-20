import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { EventService } from '../../core/services/event.service';
import { SharedEventView } from '../../core/models/event.models';

@Component({
  selector: 'app-shared-event-view',
  standalone: true,
  imports: [CommonModule, RouterModule, CurrencyPipe, DatePipe],
  templateUrl: './shared-event-view.html',
  styleUrl: './shared-event-view.scss'
})
export class SharedEventViewComponent implements OnInit {
  private eventService = inject(EventService);
  private route = inject(ActivatedRoute);

  event: SharedEventView | null = null;
  isLoading = true;
  errorMessage = '';

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const token = params.get('shareToken');
      if (token) {
        this.loadSharedEvent(token);
      } else {
        this.errorMessage = 'Invalid share link.';
        this.isLoading = false;
      }
    });
  }

  loadSharedEvent(token: string): void {
    this.isLoading = true;
    this.eventService.getSharedEvent(token).subscribe({
      next: (event) => {
        this.event = event;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Event not found or link has expired.';
        this.isLoading = false;
      }
    });
  }
}