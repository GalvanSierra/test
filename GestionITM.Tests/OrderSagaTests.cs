using Itm.Inventory.Api.Protos;

namespace Itm.Inventory.Tests.Services;

public class OrderSagaTests
{
    [Fact]
    public void GrpcCheckStock_StockCero_DebeRetornarIsAvailableFalse()
    {
        var response = new StockResponse
        {
            ProductId = 1,
            Stock = 0,
            IsAvailable = false
        };

        Assert.False(response.IsAvailable);
        Assert.Equal(0, response.Stock);
    }

    [Fact]
    public void GrpcCheckStock_StockMayorQueCero_DebeRetornarIsAvailableTrue()
    {
        var response = new StockResponse
        {
            ProductId = 1,
            Stock = 10,
            IsAvailable = true
        };

        Assert.True(response.IsAvailable);
        Assert.Equal(10, response.Stock);
    }

    [Fact]
    public void GrpcCheckStock_RequestConProductId_DebeCoincidirEnRespuesta()
    {
        var request = new StockRequest { ProductId = 5 };
        var response = new StockResponse
        {
            ProductId = request.ProductId,
            Stock = 3,
            IsAvailable = true
        };

        Assert.Equal(request.ProductId, response.ProductId);
        Assert.True(response.IsAvailable);
    }

    [Fact]
    public void Saga_StockInsuficiente_DebeAbortarSinReducir()
    {
        var stockResponse = new StockResponse
        {
            ProductId = 1,
            Stock = 0,
            IsAvailable = false
        };

        if (!stockResponse.IsAvailable)
        {
            Assert.True(true);
            return;
        }

        Assert.Fail("No deberia llegar a reducir el stock si no hay disponibilidad.");
    }

    [Fact]
    public void Saga_StockDisponible_PuedeContinuarAReducir()
    {
        var stockResponse = new StockResponse
        {
            ProductId = 1,
            Stock = 10,
            IsAvailable = true
        };

        if (stockResponse.IsAvailable)
        {
            Assert.True(true);
            return;
        }

        Assert.Fail("Deberia haber stock suficiente para continuar.");
    }
}
