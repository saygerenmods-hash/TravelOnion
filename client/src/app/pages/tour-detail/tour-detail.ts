
import { Component, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { switchMap } from 'rxjs';
import { TourService } from '../../services/tour.service';

@Component({
  selector: 'app-tour-detail',
  imports: [DatePipe, RouterLink],
  templateUrl: './tour-detail.html',
  styleUrl: './tour-detail.css'
})
export class TourDetail {
  private readonly route = inject(ActivatedRoute);
  private readonly tourService = inject(TourService);

  readonly tour = toSignal(
    this.route.paramMap.pipe(
      switchMap(params =>
        this.tourService.getTour(Number(params.get('id')))
      )
    )
  );

  getImageUrl(imagePath: string | null): string {
    return imagePath
      ? `${this.tourService.apiUrl}${imagePath}`
      : 'https://images.unsplash.com/photo-1488646953014-85cb44e25828?w=1200';
  }
}
