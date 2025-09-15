using Microsoft.VisualStudio.TestTools.UnitTesting;
using HBS;
using System;
using System.Linq;

namespace HBS.Tests
{
    [TestClass]
    public sealed class BookingServiceTests
    {
        private IBookingService _bookingService;
        private InMemoryBookingRepository _repository;
        

        [TestInitialize]
        public void Setup()
        {
            _repository = new InMemoryBookingRepository();
            _bookingService = new BookingService(_repository);
        }

        [TestMethod]
        public void CreateBooking_ValidData_ReturnsBookingWithId()
        {
            // Arrange
            var customerName = "Ridwaan Maharaj";
            var bookingType = "Flat";
            var startDate = DateTime.Today.AddDays(1);
            var endDate = DateTime.Today.AddDays(7);

            // Act
            var booking = _bookingService.CreateBooking(customerName, bookingType, startDate, endDate);

            // Assert
            Assert.IsNotNull(booking.Id);
            Assert.AreEqual(customerName, booking.CustomerName);
            Assert.AreEqual(bookingType, booking.BookingType);
            Assert.AreEqual(startDate, booking.StartDate);
            Assert.AreEqual(endDate, booking.EndDate);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateBooking_EndDateBeforeStartDate_ThrowsException()
        {
            // Arrange
            var startDate = DateTime.Today.AddDays(7);
            var endDate = DateTime.Today.AddDays(1);

            // Act & Assert
            _bookingService.CreateBooking("Ridwaan Maharaj", "Flat", startDate, endDate);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateBooking_StartDateInPast_ThrowsException()
        {
            // Arrange
            var startDate = DateTime.Today.AddDays(-1);
            var endDate = DateTime.Today.AddDays(5);

            // Act & Assert
            _bookingService.CreateBooking("Ridwaan Maharaj", "Flat", startDate, endDate);
        }

        [TestMethod]
        public void GetBooking_ExistingId_ReturnsBooking()
        {
            // Arrange
            var booking = _bookingService.CreateBooking("Ridwaan Maharaj", "Flat",
                DateTime.Today.AddDays(1), DateTime.Today.AddDays(7));

            // Act
            var result = _bookingService.GetBooking(booking.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(booking.Id, result.Id);
            Assert.AreEqual("Ridwaan Maharaj", result.CustomerName);
        }

        [TestMethod]
        public void GetBooking_NonExistingId_ReturnsNull()
        {
            // Act
            var result = _bookingService.GetBooking(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetAllBookings_ReturnsAllBookings()
        {
            // Arrange
            _bookingService.CreateBooking("Ridwaan Maharaj", "Flat",
                DateTime.Today.AddDays(1), DateTime.Today.AddDays(7));
            _bookingService.CreateBooking("Angelique Maharaj", "Vehicle",
                DateTime.Today.AddDays(2), DateTime.Today.AddDays(5));

            // Act
            var result = _bookingService.GetAllBookings();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void UpdateBooking_ExistingId_UpdatesSuccessfully()
        {
            // Arrange
            var booking = _bookingService.CreateBooking("Ridwaan Maharaj", "Flat",
                DateTime.Today.AddDays(1), DateTime.Today.AddDays(7));

            // Act
            booking.CustomerName = "Angelique Maharaj";
            _bookingService.UpdateBooking(booking);

            // Assert
            var updated = _bookingService.GetBooking(booking.Id);
            Assert.AreEqual("Angelique Maharaj", updated.CustomerName);
        }

        [TestMethod]
        public void UpdateBooking_NonExistingId_ThrowsException()
        {
            // Arrange
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                CustomerName = "Ridwaan Maharaj",
                BookingType = "Flat",
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(7)
            };

            // Act
            Exception caughtEx = null;
            try
            {
                _bookingService.UpdateBooking(booking);
            }
            catch (Exception ex)
            {
                caughtEx = ex;
            }

            // Assert
            Assert.IsNotNull(caughtEx);
            Assert.IsInstanceOfType(caughtEx, typeof(InvalidOperationException));
        }

        [TestMethod]
        public void DeleteBooking_ExistingId_RemovesFromRepository()
        {
            // Arrange
            var booking = _bookingService.CreateBooking("Ridwaan Maharaj", "Flat",
                DateTime.Today.AddDays(1), DateTime.Today.AddDays(7));

            // Act
            _bookingService.DeleteBooking(booking.Id);

            // Assert
            var deleted = _bookingService.GetBooking(booking.Id);
            Assert.IsNull(deleted);
        }

        [TestMethod]
        public void DeleteBooking_NonExistingId_DoesNotThrowException()
        {
            // Act & Assert (should not throw exception)
            _bookingService.DeleteBooking(Guid.NewGuid());
        }

        [TestMethod]
        public void Repository_AddBooking_StoresBooking()
        {
            // Arrange
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                CustomerName = "Ridwaan Maharaj",
                BookingType = "Flat",
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(7)
            };

            // Act
            _repository.Add(booking);

            // Assert
            var result = _repository.GetById(booking.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(booking.Id, result.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Repository_AddDuplicateBooking_ThrowsException()
        {
            // Arrange
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                CustomerName = "Ridwaan Maharaj",
                BookingType = "Flat",
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(7)
            };

            // Act
            _repository.Add(booking);
            _repository.Add(booking); // Add A duplicate
        }
    }
}