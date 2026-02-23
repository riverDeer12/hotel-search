using FastEndpoints;
using FluentValidation;
using HotelSearch.API.Constants;
using HotelSearch.API.Database;
using HotelSearch.API.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelSearch.API.Features.Hotels;

public sealed record CreateHotelRequest(string Name, decimal Price, double Latitude, double Longitude);
public sealed record HotelResponse(Guid Id, string Name, decimal Price, double Latitude, double Longitude);

public sealed class CreateHotelEndpoint : Endpoint<CreateHotelRequest, HotelResponse>
{
    private readonly HotelSearchContext _context;

    public CreateHotelEndpoint(HotelSearchContext context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("api/hotels");
        Options(x => x.WithTags("Hotels"));
        Summary(s =>
        {
            s.Summary = "Create a hotel";
            s.Description = "Creates a new hotel with name, price and geo coordinates.";
        });
    }

    public override async Task HandleAsync(CreateHotelRequest request, CancellationToken cancellationToken)
    {
        var newHotel = new Hotel
        {
            Name = request.Name,
            Price = request.Price,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        _context.Hotels.Add(newHotel);

        var result = await _context.SaveChangesAsync(cancellationToken);

        if (result == 0)
            ThrowError(ErrorCodes.SavingError);

        await SendAsync(
            new HotelResponse(newHotel.Id, newHotel.Name, newHotel.Price, 
                newHotel.Latitude, newHotel.Longitude),
            cancellation: cancellationToken);
    }
}

public sealed class CreateHotelValidator : Validator<CreateHotelRequest>
{
    public CreateHotelValidator(IDbContextFactory<HotelSearchContext> dbFactory)
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCodes.Required)
            .MaximumLength(200)
            .WithErrorCode(ErrorCodes.NameTooLong)
            .MustAsync(async (name, cancellationToken) =>
            {
                var normalized = name.Trim();

                await using var context = await dbFactory.CreateDbContextAsync(cancellationToken);

                return !await context.Hotels.AnyAsync(
                    hotel => hotel.Name.ToLower() == normalized.ToLower(),
                    cancellationToken);
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