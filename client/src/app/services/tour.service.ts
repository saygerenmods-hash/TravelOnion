
import { Injectable, inject } from '@angular/core';
import { HttpClient, httpResource } from '@angular/common/http';
import { Tour, TourListResponse } from '../models/tour.model';

export interface TourFilters {
  countryId: number | null;
  maxPrice: number | null;
  currentMonth: boolean;
  sortBy: string;
  page: number;
  pageSize: number;
}

export interface TourFormData {
  name: string;
  startDate: string;
  endDate: string;
  price: number;
  availablePlaces: number;
  countryId: number;
}

@Injectable({
  providedIn: 'root'
})
export class TourService {
  private readonly http = inject(HttpClient);

  readonly apiUrl = 'http://localhost:5277';

  // Получение списка туров с фильтрами и пагинацией
  getTours(filters: () => TourFilters) {
    return httpResource<TourListResponse>(() => {
      const f = filters();

      const params = new URLSearchParams({
        page: String(f.page),
        pageSize: String(f.pageSize),
        sortBy: f.sortBy,
        currentMonth: String(f.currentMonth)
      });

      if (f.countryId !== null) {
        params.set('countryId', String(f.countryId));
      }

      if (f.maxPrice !== null) {
        params.set('maxPrice', String(f.maxPrice));
      }

      return `${this.apiUrl}/api/tours?${params}`;
    });
  }

  // Получение одного тура
  getTour(id: number) {
    return this.http.get<Tour>(
      `${this.apiUrl}/api/tours/${id}`
    );
  }

  // Создание тура
  createTour(data: TourFormData) {
    return this.http.post<Tour>(
      `${this.apiUrl}/api/tours`,
      data
    );
  }

  // Редактирование тура
  updateTour(id: number, data: TourFormData) {
    return this.http.put<Tour>(
      `${this.apiUrl}/api/tours/${id}`,
      data
    );
  }

  // Удаление тура
  deleteTour(id: number) {
    return this.http.delete<void>(
      `${this.apiUrl}/api/tours/${id}`
    );
  }

  // Загрузка фотографии тура
  uploadImage(id: number, file: File) {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<Tour>(
      `${this.apiUrl}/api/tours/${id}/image`,
      formData
    );
  }
}
