
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  username = '';
  email = '';
  password = '';

  readonly loading = signal(false);
  readonly error = signal('');

  submit() {
    if (!this.username || !this.email || !this.password) {
      this.error.set('Заполни все поля.');
      return;
    }

    if (this.password.length < 8) {
      this.error.set('Пароль должен содержать минимум 8 символов.');
      return;
    }

    this.loading.set(true);
    this.error.set('');

    this.auth.register(
      this.username,
      this.email,
      this.password
    ).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/login']);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);

        const errors = err.error?.errors;

        if (errors) {
          this.error.set(
            Object.values(errors).flat().join(' ')
          );
        } else {
          this.error.set(
            err.status === 400
              ? 'Проверь данные. Возможно, пользователь уже существует.'
              : 'Не удалось зарегистрироваться.'
          );
        }
      }
    });
  }
}
