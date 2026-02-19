using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resort.Domain.Bookings;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllAsync();

    Task<List<Booking>> GetByUserIdAsync(string userId);

    Task<Booking?> FindByIdAsync(int id);

    Task<Booking> GetByIdAsync(int id);

    Task<bool> HasOverlapAsync(int placeId, DateTime fromDate, DateTime toDate);

    Task AddAsync(Booking booking);

    Task CancelAsync(int id);

    Task MarkCompletedAsync(DateTime utcNow);
}
