export interface Booking {
  id: number;
  clientId: number;
  client: {
    firstName: string;
    lastName: string;
    email: string;
  };
  maidId: number;
  maid: {
    user: {
      firstName: string;
      lastName: string;
      email: string;
    };
    hourlyRate: number;
  };
  bookingDate: string;
  durationHours: number;
  status: string;
  totalAmount: number;
  commissionAmount: number;
  notes: string;
  createdAt: string;
  updatedAt: string;
}

export interface BookingRequest {
  maidId: number;
  bookingDate: string;
  durationHours: number;
  notes?: string;
}
