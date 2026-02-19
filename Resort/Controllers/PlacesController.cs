using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Resort.Applications.Places;
using Resort.Infrastructure.Database;

namespace Resort.Controllers;

public class PlacesController : Controller
{
    private readonly AppDbContext _db;
    public PlacesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var place = await _db.Places
            .AsNoTracking()
            .Include(p => p.Features)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (place == null) return NotFound();

        var dto = new PlaceDetailsDto
        {
            Id = place.Id,
            Name = place.Name,
            City = place.City,
            Region = place.Region,
            Address = place.Address,
            Description = place.Description,
            PricePerNight = place.PricePerNight,
            GoogleMapUrl = place.GoogleMapUrl,
            ImagePaths = place.ImagePaths ?? Array.Empty<string>(),
            Features = place.Features.Select(f => new FeatureDto
            {
                Id = f.Id,
                Name = f.Name,
                IconKey = f.IconKey
            }).ToList()
        };

        return View(dto);
    }
}
