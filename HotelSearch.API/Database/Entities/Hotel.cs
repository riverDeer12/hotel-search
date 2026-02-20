using HotelSearch.API.Database.Entities.Abstract;

namespace HotelSearch.API.Database.Entities;

public class Hotel : BaseEntity
{
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
}