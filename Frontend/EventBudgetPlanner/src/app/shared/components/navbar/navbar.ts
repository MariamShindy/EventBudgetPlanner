import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class NavbarComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);

  user: { fullName: string; email: string } | null = null;
  isAuthenticated = false;

 ngOnInit(): void {
  this.isAuthenticated = this.authService.isAuthenticated();  // check token

  if (this.isAuthenticated) {
    this.user = this.authService.getCurrentUserValue();       // load user
  }

  // Listen for storage changes (e.g after login/logout)
  window.addEventListener('storage', () => {
    this.isAuthenticated = this.authService.isAuthenticated();
    this.user = this.authService.getCurrentUserValue();
  });
}


checkAuth(): void {
  this.authService.getCurrentUser().subscribe(user => {
    if (user) {
      this.user = user;
      this.isAuthenticated = true;
    } else {
      this.user = null;
      this.isAuthenticated = false;
    }
  });
}


  goToProfile(): void {
    this.router.navigate(['/auth/profile']);
  }

logout(): void {
  this.authService.logout();
  this.isAuthenticated = false;
  this.user = null;
  this.router.navigate(['/auth/login']);
}
}