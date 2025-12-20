import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../constants/api.constants';
import {
  Event,
  CreateEventRequest,
  UpdateEventRequest,
  EventFilter,
  PagedResult,
  EventSummary,
  CashflowResponse,
  ShareEventResponse,
  SharedEventView,
  AllocationResponse
} from '../models/event.models';

@Injectable({ providedIn: 'root' })
export class EventService {
  private http = inject(HttpClient);

  getAll(): Observable<Event[]> {
    return this.http.get<Event[]>(API_ENDPOINTS.EVENTS.BASE);
  }

  getById(id: number): Observable<Event> {
    return this.http.get<Event>(API_ENDPOINTS.EVENTS.BY_ID(id));
  }

  filter(filter: EventFilter): Observable<Event[]> {
  // Build clean payload - remove empty values and convert types
    const payload: any = {
    page: filter.page || 1,
    pageSize: filter.pageSize || 50,
    sortBy: filter.sortBy || 'Date',
    sortDirection: filter.sortDirection || 'desc'
  };

  // Only include non-empty values
  if (filter.searchTerm?.trim()) {
    payload.searchTerm = filter.searchTerm.trim();
  }

  if (filter.startDate) {
    // Convert date string to ISO format
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

  if (filter.minBudget !== null && filter.minBudget !== undefined) {
    payload.minBudget = Number(filter.minBudget);
  }

  if (filter.maxBudget !== null && filter.maxBudget !== undefined) {
    payload.maxBudget = Number(filter.maxBudget);
  }

  if (filter.currencyCode?.trim()) {
    payload.currencyCode = filter.currencyCode.trim();
  }

  // Backend expects EventFilterDto directly (not wrapped)
  return this.http.post<Event[]>(API_ENDPOINTS.EVENTS.FILTER, payload);
}

  create(payload: CreateEventRequest): Observable<Event> {
    return this.http.post<Event>(API_ENDPOINTS.EVENTS.BASE, payload);
  }

  update(id: number, payload: UpdateEventRequest): Observable<void> {
    return this.http.put<void>(API_ENDPOINTS.EVENTS.BY_ID(id), payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(API_ENDPOINTS.EVENTS.BY_ID(id));
  }

  getSummary(id: number): Observable<EventSummary> {
   return this.http.get<EventSummary>(`${API_ENDPOINTS.EVENTS.BY_ID(id)}/summary`);
  }

  getCashflow(id: number, interval: 'week' | 'month' = 'month'): Observable<CashflowResponse> {
  return this.http.get<CashflowResponse>(`${API_ENDPOINTS.EVENTS.BY_ID(id)}/cashflow`, {
  params: new HttpParams().set('interval', interval)
});
  }

  allocateBudget(id: number, payload: { totalBudget: number; strategy: string }): Observable<AllocationResponse> {
    return this.http.post<AllocationResponse>(`${API_ENDPOINTS.EVENTS.BY_ID(id)}/budget/allocate`, payload);
  }

  generateShareLink(id: number): Observable<ShareEventResponse> {
    return this.http.post<ShareEventResponse>(API_ENDPOINTS.EVENTS.SHARE(id), {});
  }

  getTemplateEvents(): Observable<Event[]> {
    return this.http.get<Event[]>(API_ENDPOINTS.EVENTS.TEMPLATES);
  }

  getSharedEvent(shareToken: string): Observable<SharedEventView> {
    return this.http.get<SharedEventView>(API_ENDPOINTS.SHARE.BY_TOKEN(shareToken));
  }
}