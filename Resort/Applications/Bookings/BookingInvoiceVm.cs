namespace Resort.Applications.Bookings;

public class BookingInvoiceVm
{
    public int BookingId { get; set; }

    public string InvoiceNumber { get; set; } = "";

    public string StatusLabel { get; set; } = "Confirmed";
    public string StatusCss { get; set; } = "confirmed";

    public string PaymentMethod { get; set; } = "";
    public string PaymentMethodLabel { get; set; } = "Pay Now";
    public decimal PaymentFee { get; set; }

    public string PaymentStatusTitle { get; set; } = "Payment successful";
    public string PaymentStatusSub { get; set; } = "Your payment is recorded for this booking.";
    public string IssuedAtText { get; set; } = "";

    // Guest
    public string UserId { get; set; } = "";
    public string GuestName { get; set; } = "—";
    public string GuestEmail { get; set; } = "—";

    // Place
    public string PlaceName { get; set; } = "";
    public string PlaceCity { get; set; } = "";
    public string PlaceRegion { get; set; } = "";
    public string PlaceAddress { get; set; } = "";
    public string? GoogleMapUrl { get; set; }
    public decimal PricePerNight { get; set; }

    // Stay
    public int Nights { get; set; }
    public string CheckInText { get; set; } = "";
    public string CheckOutText { get; set; } = "";

    // Totals
    public decimal SubTotal { get; set; }
    public decimal Total { get; set; }

    // Policies
    public List<string> Policies { get; set; } = new();
    public string StatusIcon { get; internal set; }
}
