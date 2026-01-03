import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { EventService } from '../../../core/services/event.service';
import { Event } from '../../../core/models/event.models';

@Component({
  selector: 'app-template-events',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './template-events.html',
  styleUrl: './template-events.scss'
})
export class TemplateEventsComponent implements OnInit {
  private eventService = inject(EventService);
  private router = inject(Router);

  templates: Event[] = [];
  isLoading = true;

  ngOnInit(): void {
    this.eventService.getTemplateEvents().subscribe({
      next: (data) => {
        this.templates = data;
        this.isLoading = false;
      },
      error: () => (this.isLoading = false)
    });
  }

  useTemplate(template: Event): void {
    const templateData = {
      name: template.name,
      date: template.date.split('T')[0],
      budget: template.budget,
      description: template.description || '',
      eventTemplateId: template.id || null
    };
    
    // Store in sessionStorage as backup
    sessionStorage.setItem('templateData', JSON.stringify(templateData));
    
    this.router.navigate(['/events/create'], {
      state: {
        template: templateData
      }
    });
  }
}