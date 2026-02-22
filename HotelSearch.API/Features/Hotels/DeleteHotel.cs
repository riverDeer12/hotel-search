using FastEndpoints;
using HotelSearch.API.Constants;
using HotelSearch.API.Database;
using Microsoft.EntityFrameworkCore;

namespace HotelSearch.API.Features.Hotels;

public sealed class DeleteHotelEndpoint : EndpointWithoutRequest<HotelResponse>
{
    private readonly HotelSearchContext _context;

    public DeleteHotelEndpoint(HotelSearchContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Delete("api/hotels/{id}");
        Options(x => x.WithTags("Hotels"));
        Summary(s =>
        {
            s.Summary = "Delete hotel";
            s.Description = "Deletes a hotel by its unique identifier.";
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

        hotel.Delete();

        _context.Hotels.Update(hotel);

        var result = await _context.SaveChangesAsync(cancellationToken);

        if (result == 0)
            ThrowError(ErrorCodes.SavingError);

        await SendAsync(
            new HotelResponse(hotel.Id, hotel.Name, hotel.Price, 
                hotel.Latitude, hotel.Longitude),
            cancellation: cancellationToken);
    }
}