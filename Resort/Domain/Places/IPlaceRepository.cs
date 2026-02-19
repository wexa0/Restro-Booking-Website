using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resort.Domain.Places;

public interface IPlaceRepository
{
    Task<List<Place>> GetAllAsync(string? filter = null);

    Task<Place?> FindByIdAsync(int id);

    Task<Place> GetByIdAsync(int id);

    Task AddAsync(Place place);
}
