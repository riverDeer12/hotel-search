using FastEndpoints;
using FluentValidation;
using HotelSearch.API.Constants;
using HotelSearch.API.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelSearch.API.Features.Hotels;

public sealed record UpdateHotelRequest(
    [property: FromRoute] Guid Id,
    string Name,
    decimal Price,
    double Latitude,
    double Longitude);

public sealed class UpdateHotelEndpoint : Endpoint<UpdateHotelRequest, HotelResponse>
{
    private readonly HotelSearchContext _context;

    public UpdateHotelEndpoint(HotelSearchContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Put("api/hotels/{id}");
        Options(x => x.WithTags("Hotels"));
        Summary(s =>
        {
            s.Summary = "Update hotel";
            s.Description = "Updates hotel details such as name, price and geo coordinates.";
            s.Params["id"] = "Hotel identifier (GUID).";
        });

    }

    public override async Task HandleAsync(UpdateHotelRequest request, CancellationToken cancellationToken)
    {
        var hotel = await _context.Hotels
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (hotel is null)
            ThrowError(ErrorCodes.NotFound);

        hotel.Name = request.Name;
        hotel.Price = request.Price;
        hotel.Latitude = request.Latitude;
        hotel.Longitude = request.Longitude;

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

public sealed class UpdateHotelValidator : Validator<UpdateHotelRequest>
{
    public UpdateHotelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode(ErrorCodes.Required)
            .MaximumLength(200)
            .WithErrorCode(ErrorCodes.NameTooLong)
            .MustAsync(async (request, name, ct) =>
            {
                var context = Resolve<HotelSearchContext>();
                
                var normalized = name.Trim();

                return !await context.Hotels.AnyAsync(hotel =>
                    hotel.Id != request.Id &&
                    hotel.Name.ToLower() == normalized.ToLower(), ct);
            })
            .WithErrorCode(ErrorCodes.AlreadyExists);

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithErrorCode(ErrorCodes.OutOfRange);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithErrorCode(ErrorCodes.OutOfRange);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithErrorCode(ErrorCodes.OutOfRange);
    }
}