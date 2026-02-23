using FluentAssertions;
using FluentValidation.TestHelper;
using HotelSearch.API.Constants;
using HotelSearch.API.Database.Entities;
using HotelSearch.API.Features.Hotels;

namespace HotelSearch.Tests.Hotels;

public class CreateHotelTests
{
     [Fact]
    public async Task Name_empty_fails_with_required()
    {
        await using var database = TestDb.CreateContext();
        
        var validator = new CreateHotelValidator();

        var response = await validator.TestValidateAsync(new CreateHotelRequest("", 10m, 0, 0));

        response.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode(ErrorCodes.Required);
    }

    [Fact]
    public async Task Name_too_long_fails_with_nameTooLong()
    {
        await using var database = TestDb.CreateContext();
        
        var validator = new CreateHotelValidator();

        var longName = new string('a', 201);

        var response = await validator.TestValidateAsync(new CreateHotelRequest(longName, 10m, 0, 0));

        response.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode(ErrorCodes.NameTooLong);
    }

    [Fact]
    public async Task Name_must_be_unique_case_insensitive_and_trimmed()
    {
        await using var database = TestDb.CreateContext();
        
        database.Hotels.Add(new Hotel { Name = "Hilton", Price = 10m, Latitude = 1, Longitude = 1 });
        
        await database.SaveChangesAsync();

        var validator = new CreateHotelValidator();

        var response = await validator.TestValidateAsync(new CreateHotelRequest("  hiLToN  ", 10m, 0, 0));

        response.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode(ErrorCodes.AlreadyExists);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Price_must_be_gt_0(decimal price)
    {
        await using var database = TestDb.CreateContext();
        
        var validator = new CreateHotelValidator();

        var response = await validator.TestValidateAsync(new CreateHotelRequest("A", price, 0, 0));

        response.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorCode(ErrorCodes.OutOfRange);
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public async Task Latitude_out_of_range_fails(double lat)
    {
       
        var validator = new CreateHotelValidator();

        var response = await validator.TestValidateAsync(new CreateHotelRequest("A", 10m, lat, 0));

        response.ShouldHaveValidationErrorFor(x => x.Latitude)
            .WithErrorCode(ErrorCodes.OutOfRange);
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public async Task Longitude_out_of_range_fails(double lng)
    {
        var validator = new CreateHotelValidator();

        var response = await validator.TestValidateAsync(new CreateHotelRequest("A", 10m, 0, lng));

        response.ShouldHaveValidationErrorFor(x => x.Longitude)
            .WithErrorCode(ErrorCodes.OutOfRange);
    }

    [Fact]
    public async Task Valid_request_passes()
    {
        var validator = new CreateHotelValidator();

        var response = await validator.TestValidateAsync(new CreateHotelRequest("Hotel", 10m, 45, 16));

        response.IsValid.Should().BeTrue();
    }
}