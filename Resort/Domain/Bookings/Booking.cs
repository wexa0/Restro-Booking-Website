using Resort.Domain.Places;

namespace Resort.Domain.Bookings;

public class Booking
{
    public int Id { get; set; }

    public int PlaceId { get; set; }
    public Place? Place { get; set; }

    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    // (RS-20260217-000123)
    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // (مثال: +50 SAR)
    public decimal PaymentFee { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Active;

    public string? UserId { get; set; }
}
