import { Component, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { BookingService } from '../../services/booking.service';
import { AuthService } from '../../services/auth.service';
import { Booking } from '../../models/booking.model';

@Component({
  selector: 'app-booking-list',
  standalone: true,
  imports: [CommonModule, RouterModule, CurrencyPipe, DatePipe],
  template: `
    <div class="container mx-auto px-4 py-8">
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-2xl font-bold">My Bookings</h1>
        @if (userRole === 'Client') {
          <a routerLink="/maids" 
            class="bg-indigo-600 text-white px-4 py-2 rounded-md hover:bg-indigo-700">
            Book a Maid
          </a>
        }
      </div>

      @if (isLoading) {
        <div class="flex justify-center items-center h-64">
          <div class="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-600"></div>
        </div>
      } @else if (bookings.length === 0) {
        <div class="text-center py-8">
          <p class="text-gray-600">No bookings found.</p>
        </div>
      } @else {
        <div class="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          @for (booking of bookings; track booking.id) {
            <div class="bg-white rounded-lg shadow-md p-6">
              <div class="flex justify-between items-start mb-4">
                <div>
                  <h3 class="font-semibold">
                    @if (userRole === 'Client') {
                      {{ booking.maid.user.firstName }} {{ booking.maid.user.lastName }}
                    } @else {
                      {{ booking.client.firstName }} {{ booking.client.lastName }}
                    }
                  </h3>
                  <p class="text-gray-600">{{ booking.bookingDate | date }}</p>
                </div>
                <span [class]="getStatusClass(booking.status)">
                  {{ booking.status }}
                </span>
              </div>
              <div class="space-y-2">
                <p><span class="font-medium">Duration:</span> {{ booking.durationHours }} hours</p>
                <p><span class="font-medium">Total:</span> {{ booking.totalAmount | currency }}</p>
                <p><span class="font-medium">Commission:</span> {{ booking.commissionAmount | currency }}</p>
                @if (booking.notes) {
                  <p><span class="font-medium">Notes:</span> {{ booking.notes }}</p>
                }
              </div>
              @if (userRole === 'Maid' && booking.status === 'Pending') {
                <div class="mt-4 space-x-2">
                  <button (click)="updateStatus(booking.id, 'Confirmed')"
                    class="bg-green-600 text-white px-3 py-1 rounded-md hover:bg-green-700">
                    Accept
                  </button>
                  <button (click)="updateStatus(booking.id, 'Cancelled')"
                    class="bg-red-600 text-white px-3 py-1 rounded-md hover:bg-red-700">
                    Decline
                  </button>
                </div>
              }
              @if (userRole === 'Maid' && booking.status === 'Confirmed') {
                <div class="mt-4">
                  <button (click)="updateStatus(booking.id, 'Completed')"
                    class="bg-indigo-600 text-white px-3 py-1 rounded-md hover:bg-indigo-700">
                    Mark as Completed
                  </button>
                </div>
              }
            </div>
          }
        </div>
      }
    </div>
  `,
  styles: []
})
export class BookingListComponent implements OnInit {
  bookings: Booking[] = [];
  isLoading = true;
  userRole: string | null = null;

  constructor(
    private bookingService: BookingService,
    private authService: AuthService
  ) {
    this.authService.currentUser$.subscribe(user => {
      this.userRole = user?.role ?? null;
    });
  }

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.bookingService.getBookings().subscribe({
      next: (bookings) => {
        this.bookings = bookings;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading bookings:', error);
        this.isLoading = false;
      }
    });
  }

  updateStatus(bookingId: number, status: string): void {
    this.bookingService.updateBookingStatus(bookingId, status).subscribe({
      next: () => {
        this.loadBookings();
      },
      error: (error) => {
        console.error('Error updating booking status:', error);
      }
    });
  }

  getStatusClass(status: string): string {
    const baseClasses = 'px-2 py-1 rounded-full text-sm font-medium';
    switch (status) {
      case 'Pending':
        return `${baseClasses} bg-yellow-100 text-yellow-800`;
      case 'Confirmed':
        return `${baseClasses} bg-blue-100 text-blue-800`;
      case 'Completed':
        return `${baseClasses} bg-green-100 text-green-800`;
      case 'Cancelled':
        return `${baseClasses} bg-red-100 text-red-800`;
      default:
        return `${baseClasses} bg-gray-100 text-gray-800`;
    }
  }
}
