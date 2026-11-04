
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

@Injectable({ providedIn: 'root' })
export class TourService {
  private readonly http = inject(HttpClient);
  readonly apiUrl = 'http://localhost:5277';

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

  getTour(id: number) {
    return this.http.get<Tour>(`${this.apiUrl}/api/tours/${id}`);
  }
}
