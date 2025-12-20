import { environment } from '../../environments/environment';

export const API_BASE_URL = environment.apiBaseUrl;

export const API_ENDPOINTS = {
  AUTH: {
    LOGIN: `${API_BASE_URL}/auth/login`,
    REGISTER: `${API_BASE_URL}/auth/register`,
    ME: `${API_BASE_URL}/auth/me`,
    FORGOT_PASSWORD: `${API_BASE_URL}/auth/forgot-password`,
    RESET_PASSWORD: `${API_BASE_URL}/auth/reset-password`,
    UPDATE_USER: `${API_BASE_URL}/auth/me`
  },
  EVENTS: {
    BASE: `${API_BASE_URL}/Events`,
    BY_ID: (id: number) => `${API_BASE_URL}/Events/${id}`,
    FILTER: `${API_BASE_URL}/Events/filter`,
    SUMMARY: (id: number) => `${API_BASE_URL}/Events/${id}/summary`,
    CASHFLOW: (id: number) => `${API_BASE_URL}/Events/${id}/cashflow`,
    ALLOCATE_BUDGET: (id: number) => `${API_BASE_URL}/Events/${id}/budget/allocate`,
    SHARE: (id: number) => `${API_BASE_URL}/Events/${id}/share`,
    TEMPLATES: `${API_BASE_URL}/Events/templates`
  },
  EXPENSES: {
    BASE: `${API_BASE_URL}/Expenses`,
    BY_ID: (id: number) => `${API_BASE_URL}/Expenses/${id}`,
    BY_EVENT: (eventId: number) => `${API_BASE_URL}/Expenses/event/${eventId}`,
    FILTER: `${API_BASE_URL}/Expenses/filter`
  },
  REMINDERS: {
    BASE: `${API_BASE_URL}/Reminders`
  },
  SHARE: {
    BY_TOKEN: (token: string) => `${API_BASE_URL}/Share/${token}`
  }
};
