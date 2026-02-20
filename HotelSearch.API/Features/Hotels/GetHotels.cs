using FastEndpoints;
using HotelSearch.API.Database;
using Microsoft.EntityFrameworkCore;

namespace HotelSearch.API.Features.Hotels;

public class GetHotelsEndpoint: EndpointWithoutRequest<List<HotelResponse>>
{
    private readonly HotelSearchContext _context;

    public GetHotelsEndpoint(HotelSearchContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Get("api/hotels");
        AllowAnonymous();
        Options(x => x.WithTags("Hotels"));
        Summary(s =>
        {
            s.Summary = "Get all hotels";
            s.Description = "Returns a list of all hotels created through the CRUD API.";
        });

    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var products = await _context.Hotels.ToListAsync(cancellationToken: cancellationToken);

        if (products.Count is 0)
        {
            await SendAsync([], cancellation: cancellationToken);
            return;
        }

        await SendAsync(products.Select(hotel => new
                HotelResponse(hotel.Id, hotel.Name, hotel.Price, 
                    hotel.Latitude, hotel.Longitude)).ToList(),
            cancellation: cancellationToken);
    }
}