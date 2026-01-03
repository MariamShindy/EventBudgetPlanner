import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, NavigationEnd, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { UserInfo } from '../../../core/models/auth.models';
import { Subscription, filter } from 'rxjs';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class ProfileComponent implements OnInit, OnDestroy {
  private authService = inject(AuthService);
  private router = inject(Router);
  private subscriptions = new Subscription();

  userInfo: UserInfo | null = null;
  isLoading: boolean = true;
  errorMessage: string = '';

  ngOnInit(): void {
    this.loadUserInfo();

    // Listen for navigation events to reload when navigating to this page
    const routerSub = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        // Reload user info when navigating to profile page
        if (event.url === '/auth/profile' || event.urlAfterRedirects === '/auth/profile') {
          this.loadUserInfo();
        }
      });
    this.subscriptions.add(routerSub);

    // Listen for auth state changes to reload user info
    const authStateListener = () => {
      this.loadUserInfo();
    };
    window.addEventListener('authStateChanged', authStateListener);
    (this as any)._authStateListener = authStateListener;
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
    if ((this as any)._authStateListener) {
      window.removeEventListener('authStateChanged', (this as any)._authStateListener);
    }
  }

  loadUserInfo(): void {
    this.isLoading = true;
    this.errorMessage = '';

    // Always fetch fresh data from API to ensure we have the latest from database
    this.authService.getCurrentUserFromApi().subscribe({
      next: (user) => {
        this.userInfo = user;
        this.isLoading = false;
      },
      error: (error) => {
        // Fallback to local storage if API call fails
        this.authService.getCurrentUser().subscribe({
          next: (user) => {
            this.userInfo = user;
            this.isLoading = false;
          },
          error: (err) => {
            this.errorMessage = err.error?.message || 'Failed to load user information.';
            this.isLoading = false;
          }
        });
      }
    });
  }
}