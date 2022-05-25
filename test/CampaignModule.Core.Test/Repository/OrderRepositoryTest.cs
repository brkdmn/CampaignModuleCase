using System.Data;
using System.Data.Common;
using CampaignModule.Core.Configuration;
using CampaignModule.Core.Repository;
using CampaignModule.Domain.Entity;
using Dapper;
using Moq;
using Moq.Dapper;

namespace CampaignModule.Core.Test.Repository;

public class OrderRepositoryTest
{
    private readonly Mock<IPostgresSqlConfiguration> _mockPostgre;
    private readonly OrderRepository _repository;

    public OrderRepositoryTest()
    {
        _mockPostgre = new Mock<IPostgresSqlConfiguration>();
        _repository = new OrderRepository(_mockPostgre.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsCampaigns()
    {
        //arrange
        var connection = new Mock<IDbConnection>();
        var mockOrder = Mock.Of<Order>();
        var expected = new[] { mockOrder };
        var tran = new Mock<DbTransaction>();

        _mockPostgre.Setup(c => c.GetConnection()).ReturnsAsync(connection.Object);
        connection.SetupDapperAsync(c => c.QueryAsync<Order>(It.IsAny<string>(), It.IsAny<object>(), tran.Object,
                It.IsAny<int>(), It.IsAny<CommandType>()))
            .ReturnsAsync(expected);

        //act
        var result = await _repository.GetAllAsync();

        //assert
        Assert.IsAssignableFrom<IEnumerable<Order>>(result);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByCodeAsync_ReturnsCampaigns()
    {
        //arrange
        var connection = new Mock<IDbConnection>();
        var mockOrder = Mock.Of<Order>();
        var expected = new[] { mockOrder };
        var tran = new Mock<DbTransaction>();

        _mockPostgre.Setup(c => c.GetConnection()).ReturnsAsync(connection.Object);
        connection.SetupDapperAsync(c => c.QueryAsync<Order>(It.IsAny<string>(), It.IsAny<object>(), tran.Object,
                It.IsAny<int>(), It.IsAny<CommandType>()))
            .ReturnsAsync(expected);

        //act
        var result = await _repository.GetByCodeAsync("750729c0-e53f-4f95-883f-f1b42cc28b06");

        //assert
        Assert.IsType<Order>(result);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetSalesCountByCampaignNameAndProductCodeAsync_ReturnsCampaigns()
    {
        //arrange
        var connection = new Mock<IDbConnection>();
        var mockQuantity = Mock.Of<TotalQuantity>();
        var expected = new[] { mockQuantity };
        var tran = new Mock<DbTransaction>();

        _mockPostgre.Setup(c => c.GetConnection()).ReturnsAsync(connection.Object);
        connection.SetupDapperAsync(c => c.QueryAsync<TotalQuantity>(It.IsAny<string>(), It.IsAny<object>(),
                tran.Object,
                It.IsAny<int>(), It.IsAny<CommandType>()))
            .ReturnsAsync(expected);

        //act
        var result = await _repository.GetSalesCountByCampaignNameAndProductCodeAsync("C1", "P1");

        //assert
        Assert.IsType<TotalQuantity>(result);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetSalesCountByProductCodeAsync_ReturnsCampaigns()
    {
        //arrange
        var connection = new Mock<IDbConnection>();
        var mockPrice = Mock.Of<TotalPrice>();
        var expected = new[] { mockPrice };
        var tran = new Mock<DbTransaction>();

        _mockPostgre.Setup(c => c.GetConnection()).ReturnsAsync(connection.Object);
        connection.SetupDapperAsync(c => c.QueryAsync<TotalPrice>(It.IsAny<string>(), It.IsAny<object>(),
                tran.Object,
                It.IsAny<int>(), It.IsAny<CommandType>()))
            .ReturnsAsync(expected);

        //act
        var result = await _repository.GetTotalPriceByCampaignNameAndProductCodeAsync("C1", "P1");

        //assert
        Assert.IsType<TotalPrice>(result);
        Assert.NotNull(result);
    }
}