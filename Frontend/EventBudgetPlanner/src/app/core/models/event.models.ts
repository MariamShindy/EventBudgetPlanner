export interface Event {
  id: number;
  name: string;
  date: string;
  budget: number;
  description?: string | null;
  createdDate: string;
}

export interface CreateEventRequest {
  name: string;
  date: string;
  budget: number;
  description?: string | null;
  currencyCode?: string;
  eventTemplateId?: number | null;
}

export interface UpdateEventRequest {
  name: string;
  date: string;
  budget: number;
  description?: string | null;
}

export interface EventFilter {
  searchTerm?: string;
  startDate?: string;
  endDate?: string;
  minBudget?: number;
  maxBudget?: number;
  currencyCode?: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface EventSummary {
  eventId: number;
  eventName: string;
  budget: number;
  totalSpent: number;
  remainingBudget: number;
  percentageSpent: number;
  expensesByCategory: Record<string, number>;
  paidExpensesCount: number;
  unpaidExpensesCount: number;
  totalExpensesCount: number;
  isOverBudget: boolean;
  overBudgetAmount: number;
}

export interface CashflowPoint {
  periodStart: string;
  periodEnd: string;
  total: number;
  cumulative: number;
}

export interface CashflowResponse {
  eventId: number;
  interval: string;
  points: CashflowPoint[];
}

export interface ShareEventResponse {
  shareUrl: string;
  shareToken: string;
}

export interface SharedEventView {
 id: number;
 name: string;
 date: string;
 budget: number;
  description?: string | null;
  totalSpent: number;
  expenseCount: number;
}

export interface TemplateEvent extends Event {
  isTemplate: boolean;
}

export interface AllocationItem {
  category: string;
  plannedAmount: number;
}

export interface AllocationResponse {
  eventId: number;
  totalBudget: number;
  strategy: string;
  allocations: AllocationItem[];
}