export interface Expense {
  id: number;
  eventId: number;
  category: string;
  description?: string | null;
  amount: number;
  paid: boolean;
  date: string;
  createdDate: string;
}

export interface CreateExpenseRequest {
  eventId: number;
  category: string;
  description?: string | null;
  amount: number;
  currencyCode?: string;
  isPaid?: boolean;
  date?: string | null;
  vendor?: string | null;
  receiptImagePath?: string | null;
  receiptFileName?: string | null;
}

export interface UpdateExpenseRequest {
  category: string;
  description?: string | null;
  amount: number;
  paid: boolean;
  date: string;
}

export interface ExpenseFilter {
  eventId?: number | null;
  category?: string | null;
  isPaid?: boolean | null;
  minAmount?: number | null;
  maxAmount?: number | null;
  startDate?: string | null;
  endDate?: string | null;
  searchTerm?: string | null;
  vendor?: string | null;
  currencyCode?: string | null;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

