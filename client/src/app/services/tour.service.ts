
import { Injectable, inject } from '@angular/core';
import { HttpClient, httpResource } from '@angular/common/http';
import { Tour, TourListResponse } from '../models/tour.model';

@Injectable({
  providedIn: 'root'
})
export class TourService {
  private readonly http = inject(HttpClient);

  readonly apiUrl = 'http://localhost:5277';

  // Получение списка туров через Angular httpResource
  getTours() {
    return httpResource<TourListResponse>(
      () => `${this.apiUrl}/api/tours?page=1&pageSize=50`
    );
  }

  // Получение одного тура по ID
  getTour(id: number) {
    return this.http.get<Tour>(
      `${this.apiUrl}/api/tours/${id}`
    );
  }
}
