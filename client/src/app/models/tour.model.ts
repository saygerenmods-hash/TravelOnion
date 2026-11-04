
export interface Tour {
  id: number;
  name: string;
  startDate: string;
  endDate: string;
  price: number;
  availablePlaces: number;
  countryId: number;
  countryName: string;
  imagePath: string | null;
  authorId: number;
}

export interface TourListResponse {
  page: number;
  pageSize: number;
  total: number;
  items: Tour[];
}
