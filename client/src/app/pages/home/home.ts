
import { DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TourService } from '../../services/tour.service';

@Component({
  selector: 'app-home',
  imports: [RouterLink, DatePipe],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  private readonly tourService = inject(TourService);

  readonly toursResource = this.tourService.getTours();

  readonly apiUrl = this.tourService.apiUrl;

  
getImageUrl(imagePath: string | null, countryId: number): string {
  // Если у тура есть загруженная фотография,
  // используем именно её
  if (imagePath) {
    return `${this.apiUrl}${imagePath}`;
  }

  // Стандартные фотографии для разных стран
  const countryImages: Record<number, string> = {
    1: 'https://images.unsplash.com/photo-1541432901042-2d8bd64b4a9b?w=900', // Турция
    2: 'https://images.unsplash.com/photo-1502602898657-3e91760cbb34?w=900', // Франция
    3: 'https://images.unsplash.com/photo-1493976040374-85c8e12f0c0e?w=900', // Япония
    4: 'https://images.unsplash.com/photo-1533105079780-92b9be482077?w=900', // Италия
    5: 'https://images.unsplash.com/photo-1543783207-ec64e4d95325?w=900'  // Испания
  };

  return countryImages[countryId] ??
    'https://images.unsplash.com/photo-1488646953014-85cb44e25828?w=900';
}

}
