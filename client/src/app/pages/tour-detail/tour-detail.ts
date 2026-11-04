
import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { switchMap } from 'rxjs';

import { TourService } from '../../services/tour.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-tour-detail',
  imports: [DatePipe, RouterLink],
  templateUrl: './tour-detail.html',
  styleUrl: './tour-detail.css'
})
export class TourDetail {
  
readonly uploading = signal(false);
readonly uploadMessage = signal('');

uploadImage(event: Event, tourId: number) {
  const input = event.target as HTMLInputElement;
  const file = input.files?.[0];

  if (!file) return;

  const allowedTypes = ['image/jpeg', 'image/png', 'image/webp'];

  if (!allowedTypes.includes(file.type)) {
    this.uploadMessage.set('Разрешены только JPG, PNG и WebP.');
    input.value = '';
    return;
  }

  if (file.size > 5 * 1024 * 1024) {
    this.uploadMessage.set('Размер фотографии не должен превышать 5 МБ.');
    input.value = '';
    return;
  }

  this.uploading.set(true);
  this.uploadMessage.set('');

  this.tourService.uploadImage(tourId, file).subscribe({
    next: () => {
      this.uploading.set(false);
      this.uploadMessage.set('Фотография успешно загружена!');
      input.value = '';

      // Обновляем данные тура, чтобы увидеть новую фотографию
      window.location.reload();
    },
    error: () => {
      this.uploading.set(false);
      this.uploadMessage.set('Не удалось загрузить фотографию.');
      input.value = '';
    }
  });
}

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly tourService = inject(TourService);

  readonly auth = inject(AuthService);
  readonly deleting = signal(false);

  // Получение информации о туре
  readonly tour = toSignal(
    this.route.paramMap.pipe(
      switchMap(params =>
        this.tourService.getTour(Number(params.get('id')))
      )
    )
  );

  // Проверка, является ли пользователь автором тура
  isAuthor(authorId: number): boolean {
    return this.auth.currentUser()?.id === authorId;
  }

  // Удаление тура
  deleteTour(id: number) {
    if (!confirm('Ты уверен, что хочешь удалить этот тур?')) {
      return;
    }

    this.deleting.set(true);

    this.tourService.deleteTour(id).subscribe({
      next: () => {
        this.deleting.set(false);
        this.router.navigate(['/']);
      },
      error: () => {
        this.deleting.set(false);
        alert('Не удалось удалить тур.');
      }
    });
  }

  // Получение фотографии тура
  getImageUrl(imagePath: string | null): string {
    return imagePath
      ? `${this.tourService.apiUrl}${imagePath}`
      : 'https://images.unsplash.com/photo-1488646953014-85cb44e25828?w=1200';
  }
}
