import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Booking, BookingRequest } from '../models/booking.model';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private readonly API_URL = 'https://localhost:7001/api'; // Update with your API port

  constructor(private http: HttpClient) {}

  getBookings(): Observable<Booking[]> {
    return this.http.get<Booking[]>(`${this.API_URL}/booking`);
  }

  getBooking(id: number): Observable<Booking> {
    return this.http.get<Booking>(`${this.API_URL}/booking/${id}`);
  }

  createBooking(booking: BookingRequest): Observable<Booking> {
    return this.http.post<Booking>(`${this.API_URL}/booking`, booking);
  }

  updateBookingStatus(id: number, status: string): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/booking/${id}/status`, JSON.stringify(status), {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}
