
import { Component, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TourFormData } from '../../services/tour.service';

@Component({
  selector: 'app-tour-form',
  imports: [FormsModule],
  templateUrl: './tour-form.html',
  styleUrl: './tour-form.css'
})
export class TourForm {
  readonly title = input('Создать путешествие');
  readonly initialData = input<TourFormData | null>(null);
  readonly saving = input(false);
  readonly serverError = input('');

  readonly submitted = output<TourFormData>();

  name = '';
  startDate = '';
  endDate = '';
  price = 0;
  availablePlaces = 1;
  countryId = 1;

  readonly error = signal('');

  ngOnChanges() {
    const data = this.initialData();

    if (data) {
      this.name = data.name;
      this.startDate = data.startDate.substring(0, 10);
      this.endDate = data.endDate.substring(0, 10);
      this.price = data.price;
      this.availablePlaces = data.availablePlaces;
      this.countryId = data.countryId;
    }
  }

  submit() {
    this.error.set('');

    if (!this.name.trim() || this.name.length > 150) {
      this.error.set('Название должно содержать от 1 до 150 символов.');
      return;
    }

    if (!this.startDate || !this.endDate) {
      this.error.set('Укажи даты путешествия.');
      return;
    }

    const days = Math.round(
      (new Date(this.endDate).getTime() -
       new Date(this.startDate).getTime()) / 86400000
    );

    if (days <= 0 || days > 30) {
      this.error.set('Продолжительность тура должна быть от 1 до 30 дней.');
      return;
    }

    if (this.price <= 0 || this.availablePlaces <= 0) {
      this.error.set('Цена и количество мест должны быть больше нуля.');
      return;
    }

    this.submitted.emit({
      name: this.name.trim(),
      startDate: this.startDate,
      endDate: this.endDate,
      price: this.price,
      availablePlaces: this.availablePlaces,
      countryId: this.countryId
    });
  }
}
