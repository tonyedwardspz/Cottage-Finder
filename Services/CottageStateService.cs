using CottageFinder.Models;

namespace CottageFinder.Services;

public class CottageStateService
{
    private SearchResults? _cachedSearchResults;
    private DateTime? _lastFetchTime;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(10);

    public SearchResults? CachedSearchResults => _cachedSearchResults;
    public bool HasValidCache => _cachedSearchResults != null && 
                                _lastFetchTime.HasValue && 
                                DateTime.Now - _lastFetchTime.Value < _cacheExpiry;

    public void SetSearchResults(SearchResults searchResults)
    {
        _cachedSearchResults = searchResults;
        _lastFetchTime = DateTime.Now;
    }

    public Cottage? GetCottageByCode(int cottageCode)
    {
        return _cachedSearchResults?.Cottages?.FirstOrDefault(c => c.Code == cottageCode);
    }

    public void ClearCache()
    {
        _cachedSearchResults = null;
        _lastFetchTime = null;
    }
}