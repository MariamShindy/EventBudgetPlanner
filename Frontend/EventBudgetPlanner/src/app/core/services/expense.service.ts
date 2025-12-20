import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../constants/api.constants';
import {
  Expense,
  CreateExpenseRequest,
  UpdateExpenseRequest,
  ExpenseFilter
} from '../models/expense.models';

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  private http = inject(HttpClient);

  // Get all expenses for a specific event
  getByEventId(eventId: number, paid?: boolean | null, category?: string | null): Observable<Expense[]> {
    let params = new HttpParams();
    if (paid !== null && paid !== undefined) {
      params = params.set('paid', paid.toString());
    }
    if (category) {
      params = params.set('category', category);
    }
    return this.http.get<Expense[]>(API_ENDPOINTS.EXPENSES.BY_EVENT(eventId), { params });
  }

  // Get expense by ID
  getById(id: number): Observable<Expense> {
    return this.http.get<Expense>(API_ENDPOINTS.EXPENSES.BY_ID(id));
  }

  // Filter expenses with advanced filtering
  filter(filter: ExpenseFilter): Observable<Expense[]> {
    // Build clean payload - remove empty values and convert types
    const payload: any = {
      page: filter.page || 1,
      pageSize: filter.pageSize || 20,
      sortBy: filter.sortBy || 'Date',
      sortDirection: filter.sortDirection || 'desc'
    };

    // Only include non-empty values
    if (filter.eventId !== null && filter.eventId !== undefined) {
      payload.eventId = filter.eventId;
    }

    if (filter.category?.trim()) {
      payload.category = filter.category.trim();
    }

    if (filter.isPaid !== null && filter.isPaid !== undefined) {
      payload.isPaid = filter.isPaid;
    }

    if (filter.minAmount !== null && filter.minAmount !== undefined) {
      payload.minAmount = Number(filter.minAmount);
    }

    if (filter.maxAmount !== null && filter.maxAmount !== undefined) {
      payload.maxAmount = Number(filter.maxAmount);
    }

    if (filter.startDate) {
      const date = new Date(filter.startDate);
      if (!isNaN(date.getTime())) {
        payload.startDate = date.toISOString();
      }
    }

    if (filter.endDate) {
      const date = new Date(filter.endDate);
      if (!isNaN(date.getTime())) {
        payload.endDate = date.toISOString();
      }
    }

    if (filter.searchTerm?.trim()) {
      payload.searchTerm = filter.searchTerm.trim();
    }

    if (filter.vendor?.trim()) {
      payload.vendor = filter.vendor.trim();
    }

    if (filter.currencyCode?.trim()) {
      payload.currencyCode = filter.currencyCode.trim();
    }

    return this.http.post<Expense[]>(API_ENDPOINTS.EXPENSES.FILTER, payload);
  }

  // Create expense
  create(payload: CreateExpenseRequest): Observable<Expense> {
    return this.http.post<Expense>(API_ENDPOINTS.EXPENSES.BASE, payload);
  }

  // Update expense
  update(id: number, payload: UpdateExpenseRequest): Observable<void> {
    return this.http.put<void>(API_ENDPOINTS.EXPENSES.BY_ID(id), payload);
  }

  // Delete expense
  delete(id: number): Observable<void> {
    return this.http.delete<void>(API_ENDPOINTS.EXPENSES.BY_ID(id));
  }
}

