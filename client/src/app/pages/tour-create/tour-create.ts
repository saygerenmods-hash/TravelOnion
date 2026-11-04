
import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { TourForm } from '../../components/tour-form/tour-form';
import { TourService, TourFormData } from '../../services/tour.service';

@Component({
  selector: 'app-tour-create',
  imports: [TourForm],
  templateUrl: './tour-create.html',
  styleUrl: './tour-create.css'
})
export class TourCreate {
  private readonly service = inject(TourService);
  private readonly router = inject(Router);

  readonly saving = signal(false);
  readonly error = signal('');

  save(data: TourFormData) {
    this.saving.set(true);
    this.error.set('');

    this.service.createTour(data).subscribe({
      next: tour => {
        this.saving.set(false);
        this.router.navigate(['/tours', tour.id]);
      },
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        this.error.set(
          err.status === 401
            ? 'Сначала войди в аккаунт.'
            : err.error?.errors
              ? Object.values(err.error.errors).flat().join(' ')
              : 'Не удалось создать тур.'
        );
      }
    });
  }
}
