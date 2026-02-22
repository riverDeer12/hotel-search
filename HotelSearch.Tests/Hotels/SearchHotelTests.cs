using FluentAssertions;
using FluentValidation.TestHelper;
using HotelSearch.API.Features.Hotels;

namespace HotelSearch.Tests.Hotels;

public class SearchHotelTests
{
    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public async Task Latitude_out_of_range_fails(double lat)
    {
        var validator = new SearchHotelsValidator();

        var response = await validator.TestValidateAsync(new SearchHotelsRequest(lat, 0));

        response.ShouldHaveValidationErrorFor(x => x.Latitude);
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public async Task Longitude_out_of_range_fails(double lng)
    {
        var validator = new SearchHotelsValidator();

        var response = await validator.TestValidateAsync(new SearchHotelsRequest(0, lng));

        response.ShouldHaveValidationErrorFor(x => x.Longitude);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Page_must_be_ge_1(int page)
    {
        var validator = new SearchHotelsValidator();

        var response = await validator.TestValidateAsync(new SearchHotelsRequest(0, 0, Page: page, PageSize: 10));

        response.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public async Task PageSize_must_be_1_to_100(int pageSize)
    {
        var validator = new SearchHotelsValidator();

        var response = await validator.TestValidateAsync(new SearchHotelsRequest(0, 0, Page: 1, PageSize: pageSize));

        response.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public async Task Valid_request_passes()
    {
        var validator = new SearchHotelsValidator();

        var response = await validator.TestValidateAsync(new SearchHotelsRequest(45, 16, 1, 10));

        response.IsValid.Should().BeTrue();
    }
}