import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../constants/api.constants';
import { CreateReminderRequest } from '../models/reminder.models';

@Injectable({
  providedIn: 'root'
})
export class ReminderService {
  private http = inject(HttpClient);

  // Send reminder
  sendReminder(request: CreateReminderRequest): Observable<void> {
    return this.http.post<void>(API_ENDPOINTS.REMINDERS.BASE, request);
  }
}

