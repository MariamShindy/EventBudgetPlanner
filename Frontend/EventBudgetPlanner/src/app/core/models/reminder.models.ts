export interface CreateReminderRequest {
  eventId: number;
  email: string;
  daysBeforeEvent: number;
  customMessage?: string | null;
}

