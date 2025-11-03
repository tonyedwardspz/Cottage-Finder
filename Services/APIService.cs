using CottageFinder.Models;

namespace CottageFinder.Services;

internal class APIService
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private static string? _currentBaseUrl;
    private static readonly object _lock = new object();

    internal string BuildUrl(DateTime? startDate = null, DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.Today;
        var end = endDate ?? DateTime.Today.AddDays(7);

        return $"https://classic.co.uk/feeds/resultsfeed.aspx?type=withCottages&dss={start:dd/MM/yy}&des={end:dd/MM/yy}&nday=7&rpp=20&pMin=1&pMax=2&rgn=CO";
    }

    internal HttpClient GetClient(string url)
    {
        lock (_lock)
        {
            if (_currentBaseUrl != url)
            {
                _httpClient.BaseAddress = new Uri(url);
                _currentBaseUrl = url;
            }

            return _httpClient;
        }
    }

    public async Task<SearchResults> GetCottages()
    {
        // Implementation to retrieve a list of cottages
        var results = new SearchResults();

        // First, load from local XML file as fallback
        try
        {
            results = await LoadCottagesFromLocalXml();
            Console.WriteLine($"Loaded {results.Cottages.Count} cottages from local XML file");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading local XML: {ex.Message}");
        }

        // Then, try to fetch from API if network is available
        // This will update the results with live data if successful
        var url = BuildUrl();

        try
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                using var client = GetClient(url);
                var response = await client.GetStringAsync(url);
                var xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.LoadXml(response);

                var apiResults = new SearchResults();

                var cottages = xmlDoc.SelectNodes("/results/cottages/cottage");
                if (cottages != null)
                {
                    foreach (System.Xml.XmlNode cottageNode in cottages)
                    {
                        apiResults.Cottages.Add(ParseCottage(cottageNode));
                    }
                }

                var totalNode = xmlDoc.SelectSingleNode("/results/total");
                if (totalNode != null && totalNode.Attributes != null)
                {
                    string totalValue = totalNode.Attributes["total"]?.Value;
                    if (int.TryParse(totalValue, out int total))
                    {
                        apiResults.Total = total;
                    }
                }

                // If we successfully got data from API, use it instead of local data
                if (apiResults.Cottages.Count > 0)
                {
                    results = apiResults;
                    Console.WriteLine($"Loaded {results.Cottages.Count} cottages from API");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from API: {ex.Message}. Using local data.");
        }

        return results;
    }

    private async Task<SearchResults> LoadCottagesFromLocalXml()
    {
        var results = new SearchResults();

        try
        {
            // Load the embedded XML file
            using var stream = await FileSystem.OpenAppPackageFileAsync("CottageData.xml");
            using var reader = new StreamReader(stream);
            var xmlContent = await reader.ReadToEndAsync();

            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            var cottages = xmlDoc.SelectNodes("/results/cottages/cottage");
            if (cottages != null)
            {
                foreach (System.Xml.XmlNode cottageNode in cottages)
                {
                    results.Cottages.Add(ParseCottage(cottageNode));
                }
            }

            var totalNode = xmlDoc.SelectSingleNode("/results/total");
            if (totalNode != null && totalNode.Attributes != null)
            {
                string totalValue = totalNode.Attributes["total"]?.Value;
                if (int.TryParse(totalValue, out int total))
                {
                    results.Total = total;
                }
            }
            else
            {
                // If no total node, set it to the count of cottages
                results.Total = results.Cottages.Count;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading local XML file: {ex.Message}");
            throw;
        }

        return results;
    }

    private static Cottage ParseCottage(System.Xml.XmlNode cottageNode)
    {
        var attributes = cottageNode.Attributes;
        var specialOffers = cottageNode.SelectNodes("specialOffers/specialOffer");
        return new Cottage
        {
            Position = int.Parse(attributes["position"]?.Value ?? "0"),
            Code = int.Parse(attributes["code"]?.Value ?? "0"),
            Year = int.Parse(attributes["year"]?.Value ?? "0"),
            Name = attributes["name"]?.Value,
            Latitude = double.Parse(attributes["lat"]?.Value ?? "0"),
            Longitude = double.Parse(attributes["lon"]?.Value ?? "0"),
            PictureUrl = attributes["picURL"]?.Value,
            Accommodation = attributes["accom"]?.Value,
            Bedrooms = int.Parse(attributes["bedrooms"]?.Value ?? "0"),
            Rent = attributes["rent"]?.Value,
            Changeover = attributes["changeover"]?.Value,
            Town = attributes["town"]?.Value,
            TownDistance = attributes["towndis"]?.Value,
            Location = attributes["location"]?.Value,
            MaxAdults = int.Parse(attributes["maxAdults"]?.Value ?? "0"),
            MinRent = decimal.Parse(attributes["minRent"]?.Value ?? "0"),
            MaxRent = decimal.Parse(attributes["maxRent"]?.Value ?? "0"),
            RentPeriod = attributes["rentPeriod"]?.Value,
            MaxPets = int.Parse(attributes["maxPets"]?.Value ?? "0"),
            IntroText = attributes["introText"]?.Value,
            SpecialOffers = specialOffers?.Cast<System.Xml.XmlNode>().Select(offer => new SpecialOffer
            {
                Title = offer.Attributes["title"]?.Value,
                Dates = offer.Attributes["dates"]?.Value,
                PriceFull = offer.Attributes["priceFull"]?.Value,
                PriceNow = offer.Attributes["priceNow"]?.Value
            }).ToList()
        };
    }
}

public class SearchResults
{
    public List<Cottage> Cottages { get; set; } = new();
    public int? Total { get; set; }
}
