using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resort.Applications.Places;

public interface IPlaceQueryService
{
    Task<List<PlaceCardDto>> GetCardsAsync(PlaceQuery query);
    Task<List<string>> GetRegionsAsync();
    Task<List<string>> GetCitiesAsync(string? region);
}
