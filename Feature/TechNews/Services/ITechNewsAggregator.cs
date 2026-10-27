using LibraryApp.Feature.TechNews.DTOs;

namespace LibraryApp.Feature.TechNews.Services;

public interface ITechNewsAggregator
{
    Task<TechNewsPagedResponse> GetTechUpdatesAsync(TechNewsQueryParameters query);
    Task<List<TechUpdateDto>> RefreshAllUpdatesAsync();
}
