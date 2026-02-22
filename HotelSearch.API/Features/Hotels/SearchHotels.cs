using FastEndpoints;
using FluentValidation;
using HotelSearch.API.Database;
using HotelSearch.API.Database.Entities;
using HotelSearch.API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace HotelSearch.API.Features.Hotels;

public sealed record SearchHotelsRequest(
    double Latitude,
    double Longitude,
    int Page = 1,
    int PageSize = 10
);

public sealed record SearchHotelsResponse(
    Guid Id,
    string Name,
    decimal Price,
    double Distance
);

public sealed record HotelDistance(Hotel Hotel, double Distance);

public sealed record HotelScore(Hotel Hotel, double Distance, double Score);

public class SearchHotelsEndpoint : Endpoint<SearchHotelsRequest, PagedResponse<SearchHotelsResponse>>
{
    private readonly HotelSearchContext _context;

    public SearchHotelsEndpoint(HotelSearchContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Get("/api/hotels/search");
        Options(x => x.WithTags("Hotels"));
        Summary(s =>
        {
            s.Summary = "Search hotels by location";
            s.Description = "Returns hotels ordered by a combined score of price and distance from the given location.";
            s.Params["lat"] = "User latitude (-90 to 90).";
            s.Params["lng"] = "User longitude (-180 to 180).";
            s.Params["page"] = "Page number (starting from 1).";
            s.Params["pageSize"] = "Number of items per page.";
        });

    }

    public override async Task HandleAsync(SearchHotelsRequest request, CancellationToken cancellationToken)
    {
        var hotels = await _context
            .Hotels.ToListAsync(cancellationToken: cancellationToken);

        if (hotels.Count is 0)
        {
            await SendAsync(new PagedResponse<SearchHotelsResponse>(
                Items: Array.Empty<SearchHotelsResponse>(),
                Page: request.Page,
                PageSize: request.PageSize,
                TotalCount: 0), cancellation: cancellationToken);
            return;
        }

        var origin = new GeoLocation(request.Latitude, request.Longitude);

        var hotelDistances = GetHotelDistances(hotels, origin);

        var scoredHotels = GetScoredHotels(hotelDistances);

        var items = scoredHotels
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new SearchHotelsResponse(
                x.Hotel.Id,
                x.Hotel.Name,
                x.Hotel.Price,
                Math.Round(x.Distance, 2)
            ))
            .ToList();

        await SendAsync(new PagedResponse<SearchHotelsResponse>(
            Items: items,
            Page: request.Page,
            PageSize: request.PageSize,
            TotalCount: hotelDistances.Count), cancellation: cancellationToken);
    }

    /// <summary>
    /// Ranks hotels by a normalized score based on price and distance (cheaper and closer first).
    /// </summary>
    /// <param name="hotelDistances">Hotels with precomputed distances from the user.</param>
    /// <returns>
    /// Ordered list of <see cref="HotelScore"/> by score, then by price and distance.
    /// </returns>
    private List<HotelScore> GetScoredHotels(List<HotelDistance> hotelDistances)
    {
        // (price/maxPrice) + (dist/maxDist)
        var maxPrice = hotelDistances.Max(x => x.Hotel.Price);
        var maxDistance = hotelDistances.Max(x => x.Distance);

        return hotelDistances
            .Select(x => new HotelScore
            (
                x.Hotel,
                x.Distance,
                (double)(x.Hotel.Price / maxPrice) + (x.Distance / maxDistance)
            ))
            .OrderBy(x => x.Score)
            .ThenBy(x => x.Hotel.Price)
            .ThenBy(x => x.Distance)
            .ToList();
    }

    private List<HotelDistance> GetHotelDistances(List<Hotel> hotels, GeoLocation origin)
    {
        var hotelDistances = hotels.Select(hotel =>
        {
            var distance = GeoLocationExtensions
                .GetDistance(origin, new GeoLocation(hotel.Latitude, hotel.Longitude));

            return new HotelDistance(hotel, distance);
        }).ToList();
        return hotelDistances;
    }
}

public sealed class SearchHotelsValidator : Validator<SearchHotelsRequest>
{
    public SearchHotelsValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}