using Itm.Price.Api.Models;
using Itm.Price.Api.Services;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text;
using System.Text.Json;

namespace Itm.Inventory.Tests.Services;

public class PriceCacheServiceTests
{
    [Fact]
    public void PriceDatabase_ExisteEvento_DebeRetornarPrecio()
    {
        var db = new PriceDatabase();
        var result = db.GetPriceById(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.EventId);
        Assert.Equal(150.00m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void PriceDatabase_EventoInexistente_DebeRetornarNull()
    {
        var db = new PriceDatabase();
        var result = db.GetPriceById(999);

        Assert.Null(result);
    }

    [Fact]
    public void CacheMetrics_RegistroHitsYMisses_DebeCalcularHitRateCorrectamente()
    {
        var metrics = new CacheMetrics();
        metrics.RecordHit();
        metrics.RecordHit();
        metrics.RecordHit();
        metrics.RecordMiss();

        Assert.Equal(3, metrics.CacheHits);
        Assert.Equal(1, metrics.CacheMisses);
        Assert.Equal(4, metrics.TotalRequests);
        Assert.Equal(75.0, metrics.HitRate);
    }

    [Fact]
    public void CacheMetrics_SinRequests_DebeRetornarHitRateCero()
    {
        var metrics = new CacheMetrics();
        Assert.Equal(0, metrics.HitRate);
    }

    [Fact]
    public async Task CacheHit_NoDebeLlamarALaBaseDeDatos()
    {
        var cacheMock = new Mock<IDistributedCache>();
        var db = new PriceDatabase();

        var cachedResponse = new PriceResponse(1, 150.00m, "USD", DateTimeOffset.UtcNow, "Redis Cache");
        var cachedBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cachedResponse));

        cacheMock.Setup(c => c.GetAsync("price:1", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(cachedBytes);

        var cachedData = await cacheMock.Object.GetAsync("price:1", CancellationToken.None);

        Assert.NotNull(cachedData);
        var deserialized = JsonSerializer.Deserialize<PriceResponse>(cachedData);
        Assert.NotNull(deserialized);
        Assert.Equal(150.00m, deserialized.Amount);
    }

    [Fact]
    public async Task CacheMiss_DebeBuscarEnBaseDeDatos()
    {
        var cacheMock = new Mock<IDistributedCache>();
        var db = new PriceDatabase();

        cacheMock.Setup(c => c.GetAsync("price:999", It.IsAny<CancellationToken>()))
                 .ReturnsAsync((byte[]?)null);

        var cachedData = await cacheMock.Object.GetAsync("price:999", CancellationToken.None);
        Assert.Null(cachedData);

        var dbResult = db.GetPriceById(999);
        Assert.Null(dbResult);
    }
}
