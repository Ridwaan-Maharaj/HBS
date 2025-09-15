using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBS
{
    public class Booking
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string BookingType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
