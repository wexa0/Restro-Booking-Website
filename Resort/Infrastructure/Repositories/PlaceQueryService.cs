using Microsoft.EntityFrameworkCore;
using Resort.Applications.Places;
using Resort.Infrastructure.Database;

namespace Resort.Infrastructure.Repositories;

public class PlaceQueryService : IPlaceQueryService
{
    private readonly AppDbContext _db;

    public PlaceQueryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<PlaceCardDto>> GetCardsAsync(PlaceQuery query)
    {
        var q = _db.Places.AsNoTracking();

        // Search
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.Trim();
            q = q.Where(p => p.Name.Contains(s) || p.City.Contains(s) || p.Region.Contains(s));
        }

        // Filters
        if (!string.IsNullOrWhiteSpace(query.Region))
            q = q.Where(p => p.Region == query.Region.Trim());

        if (!string.IsNullOrWhiteSpace(query.City))
            q = q.Where(p => p.City == query.City.Trim());

        if (query.MinPrice.HasValue)
            q = q.Where(p => p.PricePerNight >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            q = q.Where(p => p.PricePerNight <= query.MaxPrice.Value);

        // Sort
        q = query.Sort switch
        {
            PlaceSort.PriceAsc => q.OrderBy(p => p.PricePerNight),
            PlaceSort.PriceDesc => q.OrderByDescending(p => p.PricePerNight),
            _ => q.OrderByDescending(p => p.Id)
        };

        return await q.Select(p => new PlaceCardDto
        {
            Id = p.Id,
            Name = p.Name,
            City = p.City,
            Region = p.Region,
            PricePerNight = p.PricePerNight,
            MainImagePath = (p.ImagePaths != null && p.ImagePaths.Length > 0) ? p.ImagePaths[0] : ""
        }).ToListAsync();
    }

    public async Task<List<string>> GetRegionsAsync()
    {
        return await _db.Places.AsNoTracking()
            .Select(p => p.Region)
            .Where(r => r != null && r != "")
            .Distinct()
            .OrderBy(r => r)
            .ToListAsync();
    }

    public async Task<List<string>> GetCitiesAsync(string? region)
    {
        var q = _db.Places.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(region))
            q = q.Where(p => p.Region == region.Trim());

        return await q.Select(p => p.City)
            .Where(c => c != null && c != "")
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }
}
