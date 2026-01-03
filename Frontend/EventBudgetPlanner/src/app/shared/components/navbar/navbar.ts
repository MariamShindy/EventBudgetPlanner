import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { Subscription, filter } from 'rxjs';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class NavbarComponent implements OnInit, OnDestroy {
  private authService = inject(AuthService);
  private router = inject(Router);
  private subscriptions = new Subscription();

  user: { fullName: string; email: string } | null = null;
  isAuthenticated = false;

  ngOnInit(): void {
    // Initial check
    this.updateAuthState();

    // Subscribe to router navigation events to check auth state on route changes
    const routerSub = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.updateAuthState();
      });
    this.subscriptions.add(routerSub);

    // Subscribe to user changes (for cross-tab communication)
    const userSub = this.authService.getCurrentUser().subscribe(user => {
      if (user) {
        this.user = user;
        this.isAuthenticated = this.authService.isAuthenticated();
      } else {
        this.user = null;
        this.isAuthenticated = false;
      }
    });
    this.subscriptions.add(userSub);

    // Listen for storage changes (cross-tab communication)
    const storageListener = () => {
      this.updateAuthState();
    };
    window.addEventListener('storage', storageListener);
    
    // Store listener for cleanup
    (this as any)._storageListener = storageListener;

    // Listen for custom auth state change events (same-tab communication)
    const authStateListener = () => {
      this.updateAuthState();
    };
    window.addEventListener('authStateChanged', authStateListener);
    
    // Store listener for cleanup
    (this as any)._authStateListener = authStateListener;
  }

  ngOnDestroy(): void {
    // Clean up subscriptions
    this.subscriptions.unsubscribe();
    
    // Remove storage event listener
    if ((this as any)._storageListener) {
      window.removeEventListener('storage', (this as any)._storageListener);
    }
    
    // Remove auth state change event listener
    if ((this as any)._authStateListener) {
      window.removeEventListener('authStateChanged', (this as any)._authStateListener);
    }
  }

  private updateAuthState(): void {
    this.isAuthenticated = this.authService.isAuthenticated();
    if (this.isAuthenticated) {
      this.user = this.authService.getCurrentUserValue();
    } else {
      this.user = null;
    }
  }

  goToProfile(): void {
    this.router.navigate(['/auth/profile']);
  }

  logout(): void {
    this.authService.logout();
    this.updateAuthState();
    this.router.navigate(['/auth/login']);
  }
}