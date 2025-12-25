import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login';
import { RegisterComponent } from './features/auth/register/register';
import { guestGuard } from './core/guards/guest.gaurd';
import { ResetPasswordComponent } from './features/auth/reset-password/reset-password';
import { ForgotPasswordComponent } from './features/auth/forgot-password/forgot-password';
import { authGuard } from './core/guards/auth.gaurd';
import { ProfileComponent } from './features/auth/profile/profile';
import { EditProfileComponent } from './features/auth/edit-profile/edit-profile';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/auth/login',
    pathMatch: 'full',
  },
  {
    path: 'auth',
    canActivate: [guestGuard],
    children: [
      {
        path: 'login',
        component: LoginComponent
      },
      {
        path: 'register',
        component: RegisterComponent
      },
      {
        path: 'reset-password',
        component: ResetPasswordComponent
      },
      {
        path: 'forgot-password',
        component: ForgotPasswordComponent
      }
    ]
  },
  {
    path: 'share/:shareToken',
    loadComponent: () => import('./features/shared-event-view/shared-event-view').then(m => m.SharedEventViewComponent)
  },
  {
   path: 'auth',
   canActivate: [authGuard],
   children:[
   {
    path: 'profile',
    component: ProfileComponent
   },
   {
    path: 'edit-profile',
    component: EditProfileComponent
   }
   ] 
  },
  {
  path: 'events',
  canActivate: [authGuard],
  children: [
    { path: '', loadComponent: () => import('./features/events/event-list/event-list').then(m => m.EventListComponent) },
    { path: 'create', loadComponent: () => import('./features/events/event-form/event-form').then(m => m.EventFormComponent) },
    { path: ':id', loadComponent: () => import('./features/events/event-detail/event-detail').then(m => m.EventDetailComponent) },
    { path: ':id/edit', loadComponent: () => import('./features/events/event-form/event-form').then(m => m.EventFormComponent) },
    { path: ':eventId/expenses/create', loadComponent: () => import('./features/expenses/expense-form/expense-form').then(m => m.ExpenseFormComponent) },
    { path: ':eventId/expenses/:id/edit', loadComponent: () => import('./features/expenses/expense-form/expense-form').then(m => m.ExpenseFormComponent) },
    { path: 'templates/list', loadComponent: () => import('./features/events/template-events/template-events').then(m => m.TemplateEventsComponent) }
  ]
},
  {
    path: '**',
    redirectTo: '/auth/login'
  }
];
