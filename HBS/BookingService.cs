using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBS
{
    public interface IBookingService
    {
        Booking CreateBooking(string customerName, string bookingType, DateTime startDate, DateTime endDate);
        Booking? GetBooking(Guid id);
        IEnumerable<Booking> GetAllBookings();
        void UpdateBooking(Booking booking);
        void DeleteBooking(Guid id);
    }

    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repository;

        public BookingService(IBookingRepository repository) => _repository = repository;

        public Booking CreateBooking(string customerName, string bookingType, DateTime startDate, DateTime endDate)
        {
            ValidateBookingDates(startDate, endDate);

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                CustomerName = customerName,
                BookingType = bookingType,
                StartDate = startDate,
                EndDate = endDate
            };

            _repository.Add(booking);
            return booking;
        }

        public Booking? GetBooking(Guid id) => _repository.GetById(id);
        public IEnumerable<Booking> GetAllBookings() => _repository.GetAll();

        public void UpdateBooking(Booking booking)
        {
            ValidateBookingDates(booking.StartDate, booking.EndDate);

            try
            {
                _repository.Update(booking);
            }
            catch (KeyNotFoundException)
            {
                throw new InvalidOperationException("Booking not found");
            }
        }

        public void DeleteBooking(Guid id) => _repository.Delete(id);

        private static void ValidateBookingDates(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
                throw new ArgumentException("End date must be after start date");

            if (startDate < DateTime.Today)
                throw new ArgumentException("Cannot book in the past");
        }
    }
}
