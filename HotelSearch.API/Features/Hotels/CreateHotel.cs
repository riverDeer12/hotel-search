using FastEndpoints;
using FluentValidation;
using HotelSearch.API.Constants;
using HotelSearch.API.Database;
using HotelSearch.API.Database.Entities;

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
        AllowAnonymous();
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
    public CreateHotelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode(ErrorCodes.Required)
            .MaximumLength(200)
            .WithErrorCode(ErrorCodes.NameTooLong);

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