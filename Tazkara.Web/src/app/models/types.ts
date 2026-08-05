export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T | null;
  errors: string[] | null;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  token?: string;
}

export interface AuthResponse {
  token: string;
  email: string;
  role: string;
  firstName: string;
  lastName: string;
}

export interface Category {
  id: string;
  name: string;
}

export interface Event {
  id: string;
  title: string;
  description: string;
  location: string;
  startDate: string;
  endDate: string;
  capacity: number;
  availableTickets: number;
  price: number;
  categoryId: string;
  categoryName?: string;
  organizerId: string;
  status: string; // Draft, Published, Cancelled
}

export interface CreateEventRequest {
  title: string;
  description: string;
  location: string;
  startDate: string;
  endDate: string;
  capacity: number;
  price: number;
  categoryId: string;
}

export interface UpdateEventRequest extends CreateEventRequest {
  id: string;
}

export interface EventFilterRequest {
  pageNumber: number;
  pageSize: number;
  categoryId?: string;
  searchTerm?: string;
  minPrice?: number;
  maxPrice?: number;
  startDate?: string;
  endDate?: string;
  status?: string;
}

export interface Ticket {
  id: string;
  ticketNumber: string;
  eventId: string;
  eventTitle: string;
  eventDate: string;
  eventLocation: string;
  userId: string;
  status: string; // Reserved, Confirmed, Cancelled, Expired
  paymentStatus: string; // Pending, Paid, Refunded, Failed
  price: number;
  createdDate: string;
}

export interface ReserveTicketRequest {
  eventId: string;
}

export interface PaymentSessionRequest {
  ticketId: string;
  provider: number; // 0 for PayPal, 1 for VodafoneCash
}

export interface PaymentSessionResponse {
  transactionId: string;
  paymentUrl: string;
}

export interface PaymentVerificationRequest {
  transactionId: string;
  verificationToken: string;
}

export interface PaymentVerificationResponse {
  paymentId: string;
  status: string; // Paid, Failed, etc.
  ticket: Ticket;
}

export interface EventStat {
  eventId: string;
  title: string;
  startDate: string;
  status: string;
  price: number;
  capacity: number;
  ticketsSold: number;
  ticketsReserved: number;
  ticketsAvailable: number;
  revenue: number;
}

export interface OrganizerDashboardResponse {
  totalEvents: number;
  totalTicketsSold: number;
  totalRevenue: number;
  eventStats: EventStat[];
}
