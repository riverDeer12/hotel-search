
using HotelSearch.API.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HotelSearch.Tests;

public static class TestDb
{
    public static IDbContextFactory<HotelSearchContext> CreateInMemoryFactory(string dbName)
    {
        var services = new ServiceCollection();

        services.AddDbContextFactory<HotelSearchContext>(o =>
            o.UseInMemoryDatabase(dbName));

        var sp = services.BuildServiceProvider();
        return sp.GetRequiredService<IDbContextFactory<HotelSearchContext>>();
    }
}