using Microsoft.EntityFrameworkCore;
using Resort.Domain.Bookings;
using Resort.Infrastructure.Database;

namespace Resort.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _db;

    public BookingRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await _db.Bookings
            .AsNoTracking()
            .OrderByDescending(b => b.Id)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetByUserIdAsync(string userId)
    {
        return await _db.Bookings
            .AsNoTracking()
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.Id)
            .ToListAsync();
    }

    public async Task<Booking?> FindByIdAsync(int id)
    {
        return await _db.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Booking> GetByIdAsync(int id)
    {
        var booking = await FindByIdAsync(id);
        if (booking is null)
            throw new KeyNotFoundException($"Booking with id {id} was not found.");

        return booking;
    }

    public async Task<bool> HasOverlapAsync(int placeId, DateTime fromDate, DateTime toDate)
    {
        // Overlap logic: (existing.From < toDate) && (fromDate < existing.To)
        return await _db.Bookings.AnyAsync(b =>
            b.PlaceId == placeId &&
            b.Status != BookingStatus.Cancelled &&
            b.FromDate < toDate &&
            fromDate < b.ToDate
        );
    }

    public async Task AddAsync(Booking booking)
    {
        if (booking is null) throw new ArgumentNullException(nameof(booking));

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();
    }

    public async Task CancelAsync(int id)
    {
        var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == id);
        if (booking is null)
            throw new KeyNotFoundException($"Booking with id {id} was not found.");

        booking.Status = BookingStatus.Cancelled;
        await _db.SaveChangesAsync();
    }

    public async Task MarkCompletedAsync(DateTime utcNow)
    {
        // مثال: أي حجز انتهى تاريخة يصير Completed
        var toComplete = await _db.Bookings
            .Where(b => b.Status == BookingStatus.Active && b.ToDate < utcNow)

            .ToListAsync();

        foreach (var b in toComplete)
            b.Status = BookingStatus.Completed;

        await _db.SaveChangesAsync();
    }
}
