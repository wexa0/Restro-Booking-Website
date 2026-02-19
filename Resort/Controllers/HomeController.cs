using Microsoft.AspNetCore.Mvc;
using Resort.Applications.Places;

namespace Resort.Controllers;

public class HomeController : Controller
{
    private readonly IPlaceQueryService _repo;

    public HomeController(IPlaceQueryService repo)
    {
        _repo = repo;
    }

    public async Task<IActionResult> Index([FromQuery] PlaceQuery query)
    {
        var places = await _repo.GetCardsAsync(query);

        ViewBag.Regions = await _repo.GetRegionsAsync();
        ViewBag.Cities = await _repo.GetCitiesAsync(query.Region);
        ViewBag.Query = query;

        return View(places);
    }
}
