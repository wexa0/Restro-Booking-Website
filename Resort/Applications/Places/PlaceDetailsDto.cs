namespace Resort.Applications.Places;

public class PlaceDetailsDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal PricePerNight { get; set; }

    public string GoogleMapUrl { get; set; } = string.Empty;

    public string[] ImagePaths { get; set; } = Array.Empty<string>();

    public List<FeatureDto> Features { get; set; } = new();
}
