import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { UpdateUserRequest, UserInfo } from '../../../core/models/auth.models';

@Component({
  selector: 'app-edit-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './edit-profile.html',
  styleUrl: './edit-profile.scss'
})
export class EditProfileComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  editForm: FormGroup;
  currentUser: UserInfo | null = null;
  errorMessage: string = '';
  successMessage: string = '';
  isLoading: boolean = false;
  loadingUser: boolean = true;

  constructor() {
    this.editForm = this.fb.group({
      fullName: ['', [Validators.minLength(2)]],
      email: ['', [Validators.email]]
    });
  }

  ngOnInit(): void {
    this.loadCurrentUser();
  }

 
loadCurrentUser(): void {
  this.loadingUser = true;

  this.authService.getCurrentUser().subscribe({
    next: (user) => {
      if (!user) {  
        // user is null
        this.errorMessage = 'User not found.';
        this.loadingUser = false;
        return;
      }

      // user is not null ==> SAFE
      this.currentUser = user;
      this.editForm.patchValue({
        fullName: user.fullName,
        email: user.email
      });

      this.loadingUser = false;
    },
    error: (error) => {
      this.errorMessage = error.error?.message || 'Failed to load user information.';
      this.loadingUser = false;
    }
  });
}

  onSubmit(): void {
    if (this.editForm.valid && this.hasChanges()) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      const updateData: UpdateUserRequest = {
        fullName: this.editForm.value.fullName || undefined,
        email: this.editForm.value.email || undefined
      };

      this.authService.updateUser(updateData).subscribe({
        next: (updatedUser) => {
          this.isLoading = false;
          this.successMessage = 'Profile updated successfully!';
          this.currentUser = updatedUser;
          setTimeout(() => {
            this.router.navigate(['/auth/profile']);
          }, 1500);
        },
        error: (error) => {
          this.isLoading = false;
          this.errorMessage = error.error?.message || 'Failed to update profile. Please try again.';
        }
      });
    } else if (!this.hasChanges()) {
      this.errorMessage = 'No changes detected.';
    } else {
      this.markFormGroupTouched();
    }
  }

  hasChanges(): boolean {
    if (!this.currentUser) return false;
    return (
      this.editForm.value.fullName !== this.currentUser.fullName ||
      this.editForm.value.email !== this.currentUser.email
    );
  }

  private markFormGroupTouched(): void {
    Object.keys(this.editForm.controls).forEach(key => {
      const control = this.editForm.get(key);
      control?.markAsTouched();
    });
  }
}