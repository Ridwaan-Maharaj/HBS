using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBS
{
    public interface IBookingRepository
    {
        Booking? GetById(Guid id);
        IEnumerable<Booking> GetAll();
        void Add(Booking booking);
        void Update(Booking booking);
        void Delete(Guid id);
    }
}
