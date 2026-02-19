namespace Resort.Applications.Places;

public enum PlaceSort
{
    Recommended = 0,
    PriceAsc = 1,
    PriceDesc = 2
}

public class PlaceQuery
{
    public string? Search { get; set; }          // search text
    public string? Region { get; set; }          // filter region
    public string? City { get; set; }            // filter city
    public decimal? MinPrice { get; set; }       // optional
    public decimal? MaxPrice { get; set; }       // optional
    public PlaceSort Sort { get; set; } = PlaceSort.Recommended;
}
