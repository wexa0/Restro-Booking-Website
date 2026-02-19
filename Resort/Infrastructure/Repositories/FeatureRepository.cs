using Resort.Domain.Places;

namespace Resort.Infrastructure.Repositories;

public class FeatureRepository : IFeatureRepository
{
    private readonly List<Feature> _features = new();

    public List<Feature> GetAll()
    {
        return _features;
    }

    public Feature? FindById(int id)
    {
        return _features.FirstOrDefault(f => f.Id == id);
    }

    public Feature GetById(int id)
    {
        var feature = FindById(id);
        if (feature is null)
            throw new KeyNotFoundException($"Feature with id {id} was not found.");

        return feature;
    }

    public void Add(Feature feature)
    {
        if (feature is null)
            throw new ArgumentNullException(nameof(feature));

        ValidateFeature(feature);

        if (_features.Any(f => f.Id == feature.Id))
            throw new InvalidOperationException($"Feature with id {feature.Id} already exists.");

        // Optional: منع تكرار الاسم
        if (_features.Any(f => f.Name.Equals(feature.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Feature '{feature.Name}' already exists.");

        _features.Add(feature);
    }

    private static void ValidateFeature(Feature feature)
    {
        if (feature.Id <= 0)
            throw new ArgumentException("Feature.Id must be greater than 0.");

        if (string.IsNullOrWhiteSpace(feature.Name))
            throw new ArgumentException("Feature.Name is required.");

        if (string.IsNullOrWhiteSpace(feature.IconKey))
            throw new ArgumentException("Feature.IconKey is required.");
    }
}
