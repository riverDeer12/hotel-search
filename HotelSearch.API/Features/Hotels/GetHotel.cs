using FastEndpoints;
using HotelSearch.API.Constants;
using HotelSearch.API.Database;
using Microsoft.EntityFrameworkCore;

namespace HotelSearch.API.Features.Hotels;

public class GetHotelEndpoint : EndpointWithoutRequest<HotelResponse>
{
    private readonly HotelSearchContext _context;

    public GetHotelEndpoint(HotelSearchContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Get("api/hotels/{id}");
        AllowAnonymous();
        Options(x => x.WithTags("Hotels"));
        Summary(s =>
        {
            s.Summary = "Get hotel by id";
            s.Description = "Returns a single hotel by its unique identifier.";
            s.Params["id"] = "Hotel identifier (GUID).";
        });
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var hotelId = Route<Guid>("id", isRequired: true);

        var hotel =
            await _context.Hotels
                .FirstOrDefaultAsync(x => x.Id == hotelId, 
                    cancellationToken: cancellationToken);

        if (hotel is null)
            ThrowError(ErrorCodes.NotFound);

        await SendAsync(
            new HotelResponse(hotel.Id, hotel.Name, hotel.Price, 
                hotel.Latitude, hotel.Longitude),
            cancellation: cancellationToken);
    }
}