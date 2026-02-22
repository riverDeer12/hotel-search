using HotelSearch.API.Database;
using Microsoft.EntityFrameworkCore;

namespace HotelSearch.Tests;

public static class TestDb
{
    public static HotelSearchContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<HotelSearchContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new HotelSearchContext(options);
    }
}