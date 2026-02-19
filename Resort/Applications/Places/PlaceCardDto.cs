namespace Resort.Applications.Places;

public class PlaceCardDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public decimal PricePerNight { get; set; }

    public string MainImagePath { get; set; } = string.Empty;
}
