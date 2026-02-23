using FluentAssertions;
using FluentValidation.TestHelper;
using HotelSearch.API.Constants;
using HotelSearch.API.Database;
using HotelSearch.API.Database.Entities;
using HotelSearch.API.Features.Hotels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HotelSearch.Tests.Hotels;

public class UpdateHotelTests
{
    [Fact]
    public async Task Name_empty_fails_with_required()
    {
        var dbName = Guid.NewGuid().ToString();
        
        var factory = TestDb.CreateInMemoryFactory(dbName);

        var validator = new UpdateHotelValidator(factory);

        var response = await validator.TestValidateAsync(
            new UpdateHotelRequest(Guid.NewGuid(), "", 10m, 0, 0));

        response.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode(ErrorCodes.Required);
    }

    [Fact]
    public async Task Name_unique_except_same_id()
    {
        var dbName = Guid.NewGuid().ToString();
        
        var factory = TestDb.CreateInMemoryFactory(dbName);
        
        await using (var db = await factory.CreateDbContextAsync())
        {
            var a = new Hotel { Name = "A", Price = 10m, Latitude = 0, Longitude = 0 };
            
            var b = new Hotel { Name = "B", Price = 10m, Latitude = 0, Longitude = 0 };

            db.Hotels.AddRange(a, b);
            
            await db.SaveChangesAsync();
        }

        Guid aId, bId;
        
        await using (var db = await factory.CreateDbContextAsync())
        {
            aId = await db.Hotels.Where(h => h.Name == "A").Select(h => h.Id).SingleAsync();
            
            bId = await db.Hotels.Where(h => h.Name == "B").Select(h => h.Id).SingleAsync();
        }

        var validator = new UpdateHotelValidator(factory);

        var bad = await validator.TestValidateAsync(new UpdateHotelRequest(bId, "A", 10m, 0, 0));

        bad.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode(ErrorCodes.AlreadyExists);

        var ok = await validator.TestValidateAsync(new UpdateHotelRequest(aId, "A", 10m, 0, 0));

        ok.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task Price_must_be_gt_0(decimal price)
    {
        var dbName = Guid.NewGuid().ToString();
        
        var factory = TestDb.CreateInMemoryFactory(dbName);

        var validator = new UpdateHotelValidator(factory);

        var response = await validator.TestValidateAsync(
            new UpdateHotelRequest(Guid.NewGuid(), "A", price, 0, 0));

        response.ShouldHaveValidationErrorFor(x => x.Price)
            .WithErrorCode(ErrorCodes.OutOfRange);
    }

    [Fact]
    public async Task Valid_request_passes()
    {
        var dbName = Guid.NewGuid().ToString();
        
        var factory = TestDb.CreateInMemoryFactory(dbName);

        var validator = new UpdateHotelValidator(factory);

        var response = await validator.TestValidateAsync(
            new UpdateHotelRequest(Guid.NewGuid(), "Hotel", 10m, 45, 16));

        response.IsValid.Should().BeTrue();
    }
}