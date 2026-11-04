
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { TourForm } from '../../components/tour-form/tour-form';
import { TourService, TourFormData } from '../../services/tour.service';

@Component({
  selector: 'app-tour-edit',
  imports: [TourForm],
  templateUrl: './tour-edit.html',
  styleUrl: './tour-edit.css'
})
export class TourEdit {
  private readonly service = inject(TourService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly id = Number(this.route.snapshot.paramMap.get('id'));
  readonly initialData = signal<TourFormData | null>(null);
  readonly saving = signal(false);
  readonly error = signal('');

  constructor() {
    this.service.getTour(this.id).subscribe({
      next: tour => this.initialData.set({
        name: tour.name,
        startDate: tour.startDate,
        endDate: tour.endDate,
        price: tour.price,
        availablePlaces: tour.availablePlaces,
        countryId: tour.countryId
      }),
      error: () => this.error.set('Не удалось загрузить тур.')
    });
  }

  save(data: TourFormData) {
    this.saving.set(true);
    this.error.set('');

    this.service.updateTour(this.id, data).subscribe({
      next: () => {
        this.saving.set(false);
        this.router.navigate(['/tours', this.id]);
      },
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        this.error.set(
          err.status === 403
            ? 'Редактировать тур может только его автор.'
            : err.error?.errors
              ? Object.values(err.error.errors).flat().join(' ')
              : 'Не удалось сохранить изменения.'
        );
      }
    });
  }
}
