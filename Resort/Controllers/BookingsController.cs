using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Resort.Domain.Bookings;
using Resort.Infrastructure.Database;
using Resort.Applications.Bookings;
using System.Security.Claims;

public class BookingsController : Controller
{
    private readonly AppDbContext _db;
    public BookingsController(AppDbContext db) => _db = db;

    [HttpGet]
    public IActionResult Create()
        => RedirectToAction("Index", "Home");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        int placeId,
        string fromDate,
        string toDate,
        string paymentMethod
    )
    {
      

        if (string.IsNullOrWhiteSpace(fromDate) || string.IsNullOrWhiteSpace(toDate))
        {
            return RedirectToAction("Details", "Places", new { id = placeId });
        }

        if (!DateTime.TryParse(fromDate, out var checkIn) ||
            !DateTime.TryParse(toDate, out var checkOut))
        {
            return RedirectToAction("Details", "Places", new { id = placeId });
        }

        if (checkOut <= checkIn)
        {
            return RedirectToAction("Details", "Places", new { id = placeId });
        }

        var place = await _db.Places.FirstOrDefaultAsync(p => p.Id == placeId);
        if (place == null) return NotFound();

        var nights = (checkOut.Date - checkIn.Date).Days;
        if (nights < 1) nights = 1;

        paymentMethod = string.IsNullOrWhiteSpace(paymentMethod) ? "mastercard" : paymentMethod.Trim().ToLowerInvariant();
        var fee = paymentMethod == "after_checkout" ? 50m : 0m;

        var subtotal = place.PricePerNight * nights;
        var total = subtotal + fee;

        var issuedUtc = DateTime.UtcNow;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            userId = "GUEST";


        var booking = new Booking
        {
            PlaceId = placeId,
            UserId = userId,

            FromDate = checkIn,
            ToDate = checkOut,

            PaymentMethod = paymentMethod,
            PaymentFee = fee,

            CreatedAtUtc = issuedUtc,
            TotalPrice = total,

            Status = BookingStatus.Active
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        booking.InvoiceNumber = $"INV-{issuedUtc:yyyyMMdd}-{booking.Id:0000}";
        await _db.SaveChangesAsync();

        return RedirectToAction("success", new { bookingId = booking.Id });
    }

    [HttpGet]
    public IActionResult success(int bookingId)
    {
        ViewBag.BookingId = bookingId;
        return View(); 
    }
    [HttpGet]
    public async Task<IActionResult> Invoice(int id)
    {
        var booking = await _db.Bookings
            .Include(b => b.Place)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null) return NotFound();

        var user = (string.IsNullOrWhiteSpace(booking.UserId) || booking.UserId == "GUEST")
     ? null
     : await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == booking.UserId);


        var nights = (booking.ToDate - booking.FromDate).Days;
        if (nights < 1) nights = 1;

        var price = booking.Place.PricePerNight;
        var subtotal = price * nights;

        var paymentMethod = booking.PaymentMethod ?? "mastercard";
        var fee = booking.PaymentFee != 0m
            ? booking.PaymentFee
            : (paymentMethod == "after_checkout" ? 50m : 0m);


        var total = booking.TotalPrice > 0 ? booking.TotalPrice : (subtotal + fee);

        var issuedUtc = booking.CreatedAtUtc == default
    ? DateTime.UtcNow
    : booking.CreatedAtUtc;

        var invoiceNo = booking.InvoiceNumber ?? $"INV-{issuedUtc:yyyyMMdd}-{booking.Id:0000}";

        (string label, string css, string icon) UiStatus(BookingStatus s) => s switch
        {
            BookingStatus.Active => ("Active", "Active", "bi-check2-circle"),
            BookingStatus.Cancelled => ("Cancelled", "cancelled", "bi-x-circle"),
            BookingStatus.Completed => ("Completed", "completed", "bi-flag-checkered"),
            _ => ("Pending", "pending", "bi-hourglass-split")
        };
var (statusLabel, statusCss, statusIcon) = UiStatus(booking.Status);


        string MethodLabel(string m) => m switch
        {
            "mastercard" => "Mastercard (Pay Now)",
            "applepay" => "Apple Pay (Pay Now)",
            "stcpay" => "STC Pay (Pay Now)",
            "after_checkout" => "Pay later (after check-out)",
            _ => "Pay Now"
        };

        var vm = new BookingInvoiceVm
        {
            BookingId = booking.Id,

            InvoiceNumber = invoiceNo,

            StatusLabel = statusLabel,
            StatusCss = statusCss,

            PaymentMethod = paymentMethod,
            PaymentMethodLabel = MethodLabel(paymentMethod),
            PaymentFee = fee,

            IssuedAtText = issuedUtc.ToString("dd MMM yyyy, HH:mm") + " (UTC)",

            UserId = booking.UserId ?? "—",
            GuestName = User.FindFirstValue("full_name")
          ?? User.FindFirstValue(ClaimTypes.Name)
          ?? "Guest",

            GuestEmail = user?.Email ?? "—",

            PlaceName = booking.Place.Name,
            PlaceCity = booking.Place.City,
            PlaceRegion = booking.Place.Region,
            PlaceAddress = booking.Place.Address,
            GoogleMapUrl = booking.Place.GoogleMapUrl,
            PricePerNight = price,

            Nights = nights,
            CheckInText = booking.FromDate.ToString("ddd, dd MMM yyyy") + " • 09:00 AM",
            CheckOutText = booking.ToDate.ToString("ddd, dd MMM yyyy") + " • 12:00 PM",


            SubTotal = subtotal,
            Total = total,

            Policies = new List<string>
            {
                "Check-in / Check-out: Guests must follow the property check-in and check-out times.",
                "Damages: Any damages to the property or facilities may result in additional charges.",
                "Personal belongings: The property is not responsible for lost or stolen items.",
                "Capacity & visitors: Maximum capacity must be respected. Extra visitors may be restricted.",
                "Noise: Please respect quiet hours and avoid loud noise that disturbs neighbors.",
                "Cleanliness: Guests should keep the place reasonably clean. Excessive mess may incur fees.",
                "Safety: Use facilities responsibly (pool, electrical appliances, etc.). Children must be supervised.",
                "Cancellations: Cancellation policies depend on the property and booking status."
            }
        };

        if (paymentMethod == "after_checkout")
        {
            vm.PaymentStatusTitle = "Pay later selected";
            vm.PaymentStatusSub = "Payment will be collected after check-out. A 50 SAR service fee applies.";
        }
        else
        {
            vm.PaymentStatusTitle = "Payment confirmed";
            vm.PaymentStatusSub = "Your payment method was recorded for this booking.";
        }

        return View(vm);
    }
    [HttpGet]
    public async Task<IActionResult> BookedDays(int placeId, int year, int month)
    {
        var monthStart = new DateTime(year, month, 1);
        var monthEnd = monthStart.AddMonths(1);


        var bookings = await _db.Bookings.AsNoTracking()
            .Where(b =>
                b.PlaceId == placeId &&
                b.Status != BookingStatus.Cancelled &&
                b.FromDate < monthEnd &&
                b.ToDate > monthStart
            )
            .Select(b => new { b.FromDate, b.ToDate })
            .ToListAsync();

        var days = new HashSet<string>();

        foreach (var b in bookings)
        {
            var from = (b.FromDate.Date < monthStart) ? monthStart : b.FromDate.Date;
            var toExclusive = (b.ToDate.Date > monthEnd) ? monthEnd : b.ToDate.Date;

            for (var d = from; d < toExclusive; d = d.AddDays(1))
                days.Add(d.ToString("yyyy-MM-dd"));
        }

        return Json(days.OrderBy(x => x));
    }
}
