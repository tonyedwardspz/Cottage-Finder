using System.ComponentModel;

namespace CottageFinder.Models;

public partial class Cottage : INotifyPropertyChanged
{
    public int? Position { get; set; }
    public int Code { get; set; }
    public int Year { get; set; }
    public string? Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    private string? _pictureUrl;
    public string? PictureUrl
    {
        get
        {
            if (string.IsNullOrEmpty(_pictureUrl))
                return _pictureUrl;

            var baseUrl = _pictureUrl.Split('?')[0];
            return $"{baseUrl}?w=800&h=500&mode=crop";
        }
        set => _pictureUrl = value;
    }
    public string? Accommodation { get; set; }
    public int Bedrooms { get; set; }
    public string? Rent { get; set; }
    public string? Changeover { get; set; }

    private string _town { get; set; }
    public string? Town
    {
        get
        {
            if (string.IsNullOrEmpty(_town))
            {
                return Location;
            }
            else
            {
                return _town;
            }
        }
        set => _town = value;
    }

    public string? TownDistance { get; set; }
    public string? Location { get; set; }

    private int _maxAdults { get; set; }
    public int MaxAdults
    {
        get => _maxAdults;
        set
        {
            _maxAdults = value;
            OnPropertyChanged(nameof(MaxAdults));
            OnPropertyChanged(nameof(SleepsText));
        }
    }

    public string SleepsText => $"{Bedrooms} Bedroom{(Bedrooms > 1 ? "s" : "")} | {Accommodation}";

    public decimal MinRent { get; set; }
    public decimal MaxRent { get; set; }
    public string? RentPeriod { get; set; }
    public int MaxPets { get; set; }
    public string IntroText { get; set; }

    public List<SpecialOffer>? SpecialOffers { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

