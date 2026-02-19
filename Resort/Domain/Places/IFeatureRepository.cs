using System.Collections.Generic;

namespace Resort.Domain.Places;

public interface IFeatureRepository
{
    List<Feature> GetAll();

    Feature? FindById(int id);

    Feature GetById(int id);

    void Add(Feature feature);
}
