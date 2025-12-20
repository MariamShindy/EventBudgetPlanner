import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { EventService } from '../../../core/services/event.service';
import { ExpenseService } from '../../../core/services/expense.service';
import { ReminderService } from '../../../core/services/reminder.service';
import { Event, EventSummary, CashflowResponse } from '../../../core/models/event.models';
import { Expense } from '../../../core/models/expense.models';
import { CreateReminderRequest } from '../../../core/models/reminder.models';

@Component({
  selector: 'app-event-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './event-detail.html',
  styleUrl: './event-detail.scss'
})
export class EventDetailComponent implements OnInit {
  private eventService = inject(EventService);
  private expenseService = inject(ExpenseService);
  private reminderService = inject(ReminderService);
  private route = inject(ActivatedRoute);
  router = inject(Router);

  event: Event | null = null;
  summary: EventSummary | null = null;
  cashflow: CashflowResponse | null = null;
  shareLink: string | null = null;
  expenses: Expense[] = [];
  filteredExpenses: Expense[] = [];
  
  // Expense filtering
  expenseFilter = {
    paid: null as boolean | null,
    category: '' as string | null
  };
  
  // Reminder form
  reminderForm: CreateReminderRequest = {
    eventId: 0,
    email: '',
    daysBeforeEvent: 7,
    customMessage: null
  };
  showReminderForm = false;
  reminderSuccess = false;

  isLoading = true;
  errorMessage = '';
  expensesLoading = false;

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.loadEvent(+id);
      }
    });
  }
deleteEvent(): void {
  if (!this.event) { return; }
  if (!confirm('Are you sure you want to delete this event?')) { return; }

  this.isLoading = true;
  this.eventService.delete(this.event.id).subscribe({
    next: () => this.router.navigate(['/events']),
    error: err => {
      this.errorMessage = err.error?.message || 'Failed to delete event.';
      this.isLoading = false;
    }
  });
}
  loadEvent(id: number): void {
    this.isLoading = true;
    this.eventService.getById(id).subscribe({
      next: (evt) => {
        this.event = evt;
        this.isLoading = false;
        this.loadSummary(evt.id);
        this.loadCashflow(evt.id);
        this.loadExpenses(evt.id);
        this.reminderForm.eventId = evt.id;
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to load event.';
        this.isLoading = false;
      }
    });
  }

  loadExpenses(eventId: number): void {
    this.expensesLoading = true;
    this.expenseService.getByEventId(eventId).subscribe({
      next: (expenses) => {
        this.expenses = expenses;
        this.filteredExpenses = expenses;
        this.expensesLoading = false;
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to load expenses.';
        this.expensesLoading = false;
      }
    });
  }

  filterExpenses(): void {
    this.expensesLoading = true;
    const eventId = this.event?.id || 0;
    this.expenseService.getByEventId(
      eventId,
      this.expenseFilter.paid,
      this.expenseFilter.category || undefined
    ).subscribe({
      next: (expenses) => {
        this.filteredExpenses = expenses;
        this.expensesLoading = false;
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to filter expenses.';
        this.expensesLoading = false;
      }
    });
  }

  clearExpenseFilters(): void {
    this.expenseFilter = { paid: null, category: '' };
    if (this.event) {
      this.loadExpenses(this.event.id);
    }
  }

  deleteExpense(expenseId: number): void {
    if (!confirm('Are you sure you want to delete this expense?')) {
      return;
    }

    this.expenseService.delete(expenseId).subscribe({
      next: () => {
        if (this.event) {
          this.loadExpenses(this.event.id);
          this.loadSummary(this.event.id);
        }
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to delete expense.';
      }
    });
  }

  toggleExpensePaidStatus(expense: Expense): void {
    const updateRequest = {
      category: expense.category,
      description: expense.description,
      amount: expense.amount,
      paid: !expense.paid,
      date: expense.date
    };

    this.expenseService.update(expense.id, updateRequest).subscribe({
      next: () => {
        if (this.event) {
          this.loadExpenses(this.event.id);
          this.loadSummary(this.event.id);
        }
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to update expense.';
      }
    });
  }

  sendReminder(): void {
    if (!this.reminderForm.email || !this.reminderForm.daysBeforeEvent) {
      this.errorMessage = 'Please fill in all required fields.';
      return;
    }

    this.reminderService.sendReminder(this.reminderForm).subscribe({
      next: () => {
        this.reminderSuccess = true;
        this.showReminderForm = false;
        this.reminderForm = {
          eventId: this.event?.id || 0,
          email: '',
          daysBeforeEvent: 7,
          customMessage: null
        };
        setTimeout(() => {
          this.reminderSuccess = false;
        }, 3000);
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to send reminder.';
      }
    });
  }

  loadSummary(id: number): void {
    this.eventService.getSummary(id).subscribe({
      next: (summary) => (this.summary = summary)
    });
  }

  loadCashflow(id: number, interval: 'week' | 'month' = 'month'): void {
    this.eventService.getCashflow(id, interval).subscribe({
      next: (response) => (this.cashflow = response)
    });
  }

  shareToWhatsApp(): void {
  if (!this.event) {
    return;
  }

  this.eventService.generateShareLink(this.event.id).subscribe({
    next: (response) => {
      this.shareLink = response.shareUrl;
      
      // Create WhatsApp share message
      const message = `Check out this event: ${this.event?.name}\n${response.shareUrl}`;
      const encodedMessage = encodeURIComponent(message);
      
      // Open WhatsApp with the share link
      const whatsappUrl = `https://wa.me/?text=${encodedMessage}`;
      window.open(whatsappUrl, '_blank');
    },
    error: (error) => {
      this.errorMessage = error.error?.message || 'Failed to generate share link.';
    }
  });
}

  editEvent(): void {
    if (this.event) {
      this.router.navigate(['/events', this.event.id, 'edit']);
    }
  }
}