import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ExpenseService } from '../../../core/services/expense.service';
import { EventService } from '../../../core/services/event.service';
import { CreateExpenseRequest, UpdateExpenseRequest, Expense } from '../../../core/models/expense.models';
import { Event } from '../../../core/models/event.models';

@Component({
  selector: 'app-expense-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './expense-form.html',
  styleUrl: './expense-form.scss'
})
export class ExpenseFormComponent implements OnInit {
  private expenseService = inject(ExpenseService);
  private eventService = inject(EventService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  event: Event | null = null;
  expense: Expense | null = null;
  isEditMode = false;
  isLoading = false;
  errorMessage = '';

  formData: CreateExpenseRequest = {
    eventId: 0,
    category: '',
    description: null,
    amount: 0,
    currencyCode: 'USD',
    isPaid: false,
    date: new Date().toISOString().split('T')[0],
    vendor: null,
    receiptImagePath: null,
    receiptFileName: null
  };

  commonCategories = [
    'Venue',
    'Catering',
    'Entertainment',
    'Decorations',
    'Photography',
    'Transportation',
    'Accommodation',
    'Equipment',
    'Marketing',
    'Staff',
    'Other'
  ];

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const eventId = params.get('eventId');
      const expenseId = params.get('id');

      if (eventId) {
        this.formData.eventId = +eventId;
        this.loadEvent(+eventId);
      }

      if (expenseId) {
        this.isEditMode = true;
        this.loadExpense(+expenseId);
      }
    });
  }

  loadEvent(id: number): void {
    this.eventService.getById(id).subscribe({
      next: (event) => {
        this.event = event;
        // Currency code is not in Event DTO, default to USD
        this.formData.currencyCode = this.formData.currencyCode || 'USD';
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to load event.';
      }
    });
  }

  loadExpense(id: number): void {
    this.isLoading = true;
    this.expenseService.getById(id).subscribe({
      next: (expense) => {
        this.expense = expense;
        this.formData.eventId = expense.eventId;
        this.formData.category = expense.category;
        this.formData.description = expense.description || null;
        this.formData.amount = expense.amount;
        this.formData.isPaid = expense.paid;
        this.formData.date = expense.date.split('T')[0];
        this.isLoading = false;
        this.loadEvent(expense.eventId);
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to load expense.';
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (!this.validateForm()) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    if (this.isEditMode && this.expense) {
      const updateRequest: UpdateExpenseRequest = {
        category: this.formData.category,
        description: this.formData.description,
        amount: this.formData.amount,
        paid: this.formData.isPaid || false,
        date: new Date(this.formData.date || new Date()).toISOString()
      };

      this.expenseService.update(this.expense.id, updateRequest).subscribe({
        next: () => {
          this.router.navigate(['/events', this.formData.eventId]);
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Failed to update expense.';
          this.isLoading = false;
        }
      });
    } else {
      const createRequest: CreateExpenseRequest = {
        ...this.formData,
        date: new Date(this.formData.date || new Date()).toISOString()
      };

      this.expenseService.create(createRequest).subscribe({
        next: () => {
          this.router.navigate(['/events', this.formData.eventId]);
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Failed to create expense.';
          this.isLoading = false;
        }
      });
    }
  }

  validateForm(): boolean {
    if (!this.formData.category || this.formData.category.trim() === '') {
      this.errorMessage = 'Category is required.';
      return false;
    }
    if (!this.formData.amount || this.formData.amount <= 0) {
      this.errorMessage = 'Amount must be greater than 0.';
      return false;
    }
    if (!this.formData.date) {
      this.errorMessage = 'Date is required.';
      return false;
    }
    return true;
  }

  cancel(): void {
    this.router.navigate(['/events', this.formData.eventId]);
  }
}

