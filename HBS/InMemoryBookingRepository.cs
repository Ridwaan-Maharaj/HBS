using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBS
{
    public class InMemoryBookingRepository : IBookingRepository
    {
        private readonly ConcurrentDictionary<Guid, Booking> _bookings = new();

        public Booking? GetById(Guid id) => _bookings.GetValueOrDefault(id);
        public IEnumerable<Booking> GetAll() => _bookings.Values;

        public void Add(Booking booking)
        {
            if (!_bookings.TryAdd(booking.Id, booking))
                throw new InvalidOperationException("Booking already exists");
        }

        public void Update(Booking booking)
        {
            if (!_bookings.ContainsKey(booking.Id))
                throw new InvalidOperationException("Booking not found");

            _bookings[booking.Id] = booking;
        }

        public void Delete(Guid id) => _bookings.TryRemove(id, out _);
    }
}
