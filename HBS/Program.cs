using HBS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            Console.Title = "Holiday Booking System";
            Console.CursorVisible = false;

            var services = new ServiceCollection();
            ConfigureServices(services);
            var provider = services.BuildServiceProvider();

            var bookingService = provider.GetRequiredService<IBookingService>();
            var consoleUI = new ConsoleUI(bookingService);
            consoleUI.Run();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Fatal error: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        finally
        {
            Console.CursorVisible = true;
        }
    }

    static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();
        services.AddScoped<IBookingService, BookingService>();
    }
}

public class ConsoleUI
{
    private readonly IBookingService _bookingService;

    public ConsoleUI(IBookingService bookingService) => _bookingService = bookingService;

    public void Run()
    {
        Console.Clear();
        DisplayHeader();

        while (true)
        {
            try
            {
                DisplayMenu();
                var choice = GetMenuChoice();

                switch (choice)
                {
                    case 1: CreateBooking(); break;
                    case 2: ViewBookings(); break;
                    case 3: UpdateBooking(); break;
                    case 4: DeleteBooking(); break;
                    case 5:
                        Console.Clear();
                        DisplayFarewell();
                        return;
                    default:
                        ShowError("Invalid option. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Unexpected error: {ex.Message}");
            }
        }
    }

    private void DisplayHeader()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;

        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  *   *   *   *   *   *   *   *   *   *   *   *   *   *   *   ║");
        Console.WriteLine("║                                                              ║");
        Console.WriteLine("║                 HOLIDAY BOOKING SYSTEM                       ║");
        Console.WriteLine("║                     WINTER FLOOD                             ║");
        Console.WriteLine("║                                                              ║");
        Console.WriteLine("║  *   *   *   *   *   *   *   *   *   *   *   *   *   *   *   ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

        Console.ResetColor();
        Console.WriteLine();
    }

    private void DisplayMenu()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                          MAIN MENU                           ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
        Console.WriteLine("║  1. Create a new booking                                     ║");
        Console.WriteLine("║  2. View all bookings                                        ║");
        Console.WriteLine("║  3. Update an existing booking                               ║");
        Console.WriteLine("║  4. Delete a booking                                         ║");
        Console.WriteLine("║  5. Exit application                                         ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
        Console.Write("Select an option (1-5): ");
    }

    private int GetMenuChoice()
    {
        while (true)
        {
            var input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= 5)
            {
                return choice;
            }
            ShowError("Invalid input. Please enter a number between 1 and 5.");
            Console.Write("Select an option (1-5): ");
        }
    }

    private void CreateBooking()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                     CREATE NEW BOOKING                       ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        try
        {
            var booking = ReadBookingDetails();
            var created = _bookingService.CreateBooking(
                booking.CustomerName,
                booking.BookingType,
                booking.StartDate,
                booking.EndDate);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  BOOKING CREATED SUCCESSFULLY                ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine($"Booking ID: {created.Id}");
            Console.WriteLine($"Customer: {created.CustomerName}");
            Console.WriteLine($"Type: {created.BookingType}");
            Console.WriteLine($"Dates: {created.StartDate:d} to {created.EndDate:d}");
        }
        catch (Exception ex)
        {
            ShowError($"Error creating booking: {ex.Message}");
        }
        finally
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
            DisplayHeader();
        }
    }

    private void ViewBookings()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                      ALL BOOKINGS                            ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        try
        {
            var bookings = _bookingService.GetAllBookings().ToList();

            if (!bookings.Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  No bookings found.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║ ID                                   Customer          Type        Start Date    End Date                ║");
                Console.WriteLine("╠══════════════════════════════════════════════════════════════════════════════════════════════════════════╣");

                foreach (var b in bookings)
                {
                    Console.WriteLine($"║ {b.Id} {b.CustomerName,-15} {b.BookingType,-12} {b.StartDate:d}    {b.EndDate:d}    ║");
                }

                Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════════════════════════════════╝");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error retrieving bookings: {ex.Message}");
        }
        finally
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
            DisplayHeader();
        }
    }

    private void UpdateBooking()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                     UPDATE BOOKING                          ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        try
        {
            Console.Write("Enter booking ID to update: ");
            if (!Guid.TryParse(Console.ReadLine(), out var id))
            {
                ShowError("Invalid ID format");
                return;
            }

            var existing = _bookingService.GetBooking(id);
            if (existing == null)
            {
                ShowError("Booking not found");
                return;
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Current booking details:");
            Console.ResetColor();
            Console.WriteLine($"Customer: {existing.CustomerName}");
            Console.WriteLine($"Type: {existing.BookingType}");
            Console.WriteLine($"Dates: {existing.StartDate:d} to {existing.EndDate:d}");
            Console.WriteLine();

            var updated = ReadBookingDetails();
            updated.Id = id;

            _bookingService.UpdateBooking(updated);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  BOOKING UPDATED SUCCESSFULLY               ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            ShowError($"Error updating booking: {ex.Message}");
        }
        finally
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
            DisplayHeader();
        }
    }

    private void DeleteBooking()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                     DELETE BOOKING                          ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        try
        {
            Console.Write("Enter booking ID to delete: ");
            if (!Guid.TryParse(Console.ReadLine(), out var id))
            {
                ShowError("Invalid ID format");
                return;
            }

            var existing = _bookingService.GetBooking(id);
            if (existing == null)
            {
                ShowError("Booking not found");
                return;
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Booking to be deleted:");
            Console.ResetColor();
            Console.WriteLine($"Customer: {existing.CustomerName}");
            Console.WriteLine($"Type: {existing.BookingType}");
            Console.WriteLine($"Dates: {existing.StartDate:d} to {existing.EndDate:d}");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Are you sure you want to delete this booking? (y/N): ");
            Console.ResetColor();

            var confirmation = Console.ReadLine();
            if (confirmation?.ToLower() != "y")
            {
                Console.WriteLine();
                Console.WriteLine("Deletion cancelled.");
                return;
            }

            _bookingService.DeleteBooking(id);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  BOOKING DELETED SUCCESSFULLY               ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            ShowError($"Error deleting booking: {ex.Message}");
        }
        finally
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
            DisplayHeader();
        }
    }

    private static Booking ReadBookingDetails()
    {
        string name;
        do
        {
            Console.Write("Customer Name: ");
            name = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(name))
            {
                ShowError("Customer name is required");
            }
        } while (string.IsNullOrEmpty(name));

        string type;
        do
        {
            Console.Write("Booking Type (apartment, vehicle, show, etc.): ");
            type = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(type))
            {
                ShowError("Booking type is required");
            }
        } while (string.IsNullOrEmpty(type));

        DateTime startDate;
        while (true)
        {
            Console.Write("Start Date (yyyy-mm-dd): ");
            var startInput = Console.ReadLine();

            if (DateTime.TryParseExact(startInput, "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            {
                if (startDate >= DateTime.Today)
                {
                    break;
                }
                ShowError("Start date cannot be in the past.");
            }
            else
            {
                ShowError("Invalid date format. Please use yyyy-mm-dd format.");
            }
        }

        DateTime endDate;
        while (true)
        {
            Console.Write("End Date (yyyy-mm-dd): ");
            var endInput = Console.ReadLine();

            if (DateTime.TryParseExact(endInput, "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                if (endDate > startDate)
                {
                    break;
                }
                ShowError("End date must be after start date.");
            }
            else
            {
                ShowError("Invalid date format. Please use yyyy-mm-dd format.");
            }
        }

        return new Booking
        {
            CustomerName = name,
            BookingType = type,
            StartDate = startDate,
            EndDate = endDate
        };
    }

    private static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {message}");
        Console.ResetColor();
    }

    private void DisplayFarewell()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                                                              ║");
        Console.WriteLine("║                 Thank you for using                         ║");
        Console.WriteLine("║                 Holiday Booking System                      ║");
        Console.WriteLine("║                                                              ║");
        Console.WriteLine("║                 Have a great day!                           ║");
        Console.WriteLine("║                                                              ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }
}