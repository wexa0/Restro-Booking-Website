using System.Text.Json.Serialization;

namespace Resort.Domain.Places
{
    public class Place
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

        public List<Feature> Features { get; set; } = new();

    }
}