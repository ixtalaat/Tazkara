import { Routes } from '@angular/router';
import { BrowseEventsComponent } from './pages/browse-events/browse-events';
import { LoginComponent } from './pages/login/login';
import { RegisterComponent } from './pages/register/register';
import { EventDetailsComponent } from './pages/event-details/event-details';
import { MyTicketsComponent } from './pages/my-tickets/my-tickets';
import { OrganizerDashboardComponent } from './pages/dashboard/dashboard';
import { CreateEventComponent } from './pages/create-event/create-event';
import { PaymentCheckoutComponent } from './pages/payment/payment-checkout';
import { AdminDashboardComponent } from './pages/admin-dashboard/admin-dashboard';
import { authGuard, noAuthGuard } from './services/auth.guard';

export const routes: Routes = [
  { path: '', component: BrowseEventsComponent },
  { path: 'login', component: LoginComponent, canActivate: [noAuthGuard] },
  { path: 'register', component: RegisterComponent, canActivate: [noAuthGuard] },
  { path: 'events/:id', component: EventDetailsComponent },
  { path: 'my-tickets', component: MyTicketsComponent, canActivate: [authGuard], data: { roles: ['Customer'] } },
  { path: 'dashboard', component: OrganizerDashboardComponent, canActivate: [authGuard], data: { roles: ['Organizer'] } },
  { path: 'admin', component: AdminDashboardComponent, canActivate: [authGuard], data: { roles: ['Admin'] } },
  { path: 'dashboard/events/create', component: CreateEventComponent, canActivate: [authGuard], data: { roles: ['Organizer'] } },
  { path: 'dashboard/events/edit/:id', component: CreateEventComponent, canActivate: [authGuard], data: { roles: ['Organizer'] } },
  { path: 'payment/checkout', component: PaymentCheckoutComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: '' }
];
