using FluentAssertions;
using FluentValidation.TestHelper;
using HotelSearch.API.Constants;
using HotelSearch.API.Database.Entities;
using HotelSearch.API.Features.Hotels;

namespace HotelSearch.Tests.Hotels;

public class UpdateHotelTests
{
    [Fact]
    public async Task Name_empty_fails_with_required()
    {
        var validator = new UpdateHotelValidator();

        var response = await validator.TestValidateAsync(new UpdateHotelRequest(Guid.NewGuid(), "", 10m, 0, 0));

        response.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode(ErrorCodes.Required);
    }

    [Fact]
    public async Task Name_unique_except_same_id()
    {
        await using var database = TestDb.CreateContext();

        var a = new Hotel { Name = "A", Price = 10m, Latitude = 0, Longitude = 0 };
        var b = new Hotel { Name = "B", Price = 10m, Latitude = 0, Longitude = 0 };

        database.Hotels.AddRange(a, b);

        await database.SaveChangesAsync();

        var validator = new UpdateHotelValidator();

        // renaming B -> A should fail
        var bad = await validator.TestValidateAsync(new UpdateHotelRequest(b.Id, "A", 10m, 0, 0));

        bad.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode(ErrorCodes.AlreadyExists);

        // updating A with same name should pass
        var ok = await validator.TestValidateAsync(new UpdateHotelRequest(a.Id, "A", 10m, 0, 0));

        ok.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task Price_must_be_gt_0(decimal price)
    {
        var validator = new UpdateHotelValidator();

        var response = await validator.TestValidateAsync(new UpdateHotelRequest(Guid.NewGuid(), "A", price, 0, 0));

        response.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorCode(ErrorCodes.OutOfRange);
    }

    [Fact]
    public async Task Valid_request_passes()
    {
        var validator = new UpdateHotelValidator();

        var response = await validator.TestValidateAsync(new UpdateHotelRequest(Guid.NewGuid(), "Hotel", 10m, 45, 16));

        response.IsValid.Should().BeTrue();
    }
}