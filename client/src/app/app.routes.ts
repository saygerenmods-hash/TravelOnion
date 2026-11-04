
import { Routes } from '@angular/router';

import { Home } from './pages/home/home';
import { TourDetail } from './pages/tour-detail/tour-detail';
import { TourCreate } from './pages/tour-create/tour-create';
import { TourEdit } from './pages/tour-edit/tour-edit';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { NotFound } from './pages/not-found/not-found';

export const routes: Routes = [
  { path: '', component: Home, title: 'TravelOnion — Путешествия' },
  { path: 'tours/create', component: TourCreate, title: 'Создать тур' },
  { path: 'tours/:id/edit', component: TourEdit, title: 'Редактировать тур' },
  { path: 'tours/:id', component: TourDetail, title: 'Информация о туре' },
  { path: 'login', component: Login, title: 'Вход' },
  { path: 'register', component: Register, title: 'Регистрация' },
  { path: '**', component: NotFound, title: 'Страница не найдена' }
];
