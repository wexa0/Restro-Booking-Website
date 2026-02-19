namespace Resort.Applications.Places;

public interface IPlaceRepository
{
    Task<List<PlaceCardDto>> GetAllAsync(PlaceQuery query);
    Task<List<string>> GetRegionsAsync();
    Task<List<string>> GetCitiesAsync(string? region);
}
