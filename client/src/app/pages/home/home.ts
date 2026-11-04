
import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TourService, TourFilters } from '../../services/tour.service';

@Component({
  selector: 'app-home',
  imports: [RouterLink, DatePipe],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
   readonly Math = Math;
  private readonly tourService = inject(TourService);

  readonly apiUrl = this.tourService.apiUrl;

  readonly filters = signal<TourFilters>({
    countryId: null,
    maxPrice: null,
    currentMonth: false,
    sortBy: 'date',
    page: 1,
    pageSize: 6
  });

  readonly toursResource = this.tourService.getTours(() => this.filters());

  setCountry(value: string) {
    this.filters.update(f => ({
      ...f,
      countryId: value ? Number(value) : null,
      page: 1
    }));
  }

  setMaxPrice(value: string) {
    this.filters.update(f => ({
      ...f,
      maxPrice: value ? Number(value) : null,
      page: 1
    }));
  }

  setSort(value: string) {
    this.filters.update(f => ({
      ...f,
      sortBy: value,
      page: 1
    }));
  }

  setCurrentMonth(checked: boolean) {
    this.filters.update(f => ({
      ...f,
      currentMonth: checked,
      page: 1
    }));
  }

  changePage(page: number) {
    this.filters.update(f => ({ ...f, page }));
  }

  getImageUrl(imagePath: string | null, countryId: number): string {
    if (imagePath) {
      return `${this.apiUrl}${imagePath}`;
    }

    const images: Record<number, string> = {
      1: 'https://images.unsplash.com/photo-1541432901042-2d8bd64b4a9b?w=900',
      2: 'https://images.unsplash.com/photo-1502602898657-3e91760cbb34?w=900',
      3: 'https://images.unsplash.com/photo-1493976040374-85c8e12f0c0e?w=900',
      4: 'https://images.unsplash.com/photo-1533105079780-92b9be482077?w=900',
      5: 'https://images.unsplash.com/photo-1543783207-ec64e4d95325?w=900'
    };

    return images[countryId] ??
      'https://images.unsplash.com/photo-1488646953014-85cb44e25828?w=900';
  }
}
