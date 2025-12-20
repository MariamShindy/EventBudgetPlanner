import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { UserInfo } from '../../../core/models/auth.models';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class ProfileComponent implements OnInit {
  private authService = inject(AuthService);

  userInfo: UserInfo | null = null;
  isLoading: boolean = true;
  errorMessage: string = '';

  ngOnInit(): void {
    this.loadUserInfo();
  }

  loadUserInfo(): void {
    this.isLoading = true;
    this.errorMessage = '';

    // Try to get from API first, fallback to local storage
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