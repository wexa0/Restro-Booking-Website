using Microsoft.EntityFrameworkCore;
using Resort.Domain.Places;
using Resort.Infrastructure.Database;

namespace Resort.Infrastructure.Repositories;

public class PlaceRepository : IPlaceRepository
{
    private readonly AppDbContext _db;

    public PlaceRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Place>> GetAllAsync(string? filter = null)
    {
        var query = _db.Places.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var f = filter.Trim();
            query = query.Where(p =>
                p.Name.Contains(f) ||
                p.City.Contains(f) ||
                p.Region.Contains(f));
        }

        return await query
            .OrderByDescending(p => p.Id)
            .ToListAsync();
    }

    public async Task<Place?> FindByIdAsync(int id)
    {
        return await _db.Places
            .AsNoTracking()
            .Include(p => p.Features)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Place> GetByIdAsync(int id)
    {
        var place = await FindByIdAsync(id);
        if (place is null)
            throw new KeyNotFoundException($"Place with id {id} was not found.");

        return place;
    }

    public async Task AddAsync(Place place)
    {
        if (place is null) throw new ArgumentNullException(nameof(place));

        // ✅ لا تفحصين Id > 0 لأن Id Identity بالداتابيس
        if (string.IsNullOrWhiteSpace(place.Name))
            throw new ArgumentException("Place.Name is required.");
        if (string.IsNullOrWhiteSpace(place.City))
            throw new ArgumentException("Place.City is required.");
        if (string.IsNullOrWhiteSpace(place.Region))
            throw new ArgumentException("Place.Region is required.");
        if (place.PricePerNight <= 0)
            throw new ArgumentException("Place.PricePerNight must be greater than 0.");

        _db.Places.Add(place);
        await _db.SaveChangesAsync();
    }
}
