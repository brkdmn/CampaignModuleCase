using System.Data;
using System.Data.Common;
using CampaignModule.Core.Configuration;
using CampaignModule.Core.Repository;
using CampaignModule.Domain.Entity;
using Dapper;
using Moq;
using Moq.Dapper;

namespace CampaignModule.Core.Test.Repository;

public class ProductRepositoryTest
{
    private readonly Mock<IPostgresSqlConfiguration> _mockPostgre;
    private readonly ProductRepository _repository;

    public ProductRepositoryTest()
    {
        _mockPostgre = new Mock<IPostgresSqlConfiguration>();
        _repository = new ProductRepository(_mockPostgre.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsCampaigns()
    {
        //arrange
        var connection = new Mock<IDbConnection>();
        var mockProduct = Mock.Of<Product>();
        var expected = new[] { mockProduct };
        var tran = new Mock<DbTransaction>();

        _mockPostgre.Setup(c => c.GetConnection()).ReturnsAsync(connection.Object);
        connection.SetupDapperAsync(c => c.QueryAsync<Product>(It.IsAny<string>(), It.IsAny<object>(), tran.Object,
                It.IsAny<int>(), It.IsAny<CommandType>()))
            .ReturnsAsync(expected);

        //act
        var result = await _repository.GetAllAsync();

        //assert
        Assert.IsAssignableFrom<IEnumerable<Product>>(result);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByCodeAsync_ReturnsCampaigns()
    {
        //arrange
        var connection = new Mock<IDbConnection>();
        var mockProduct = Mock.Of<Product>();
        var expected = new[] { mockProduct };
        var tran = new Mock<DbTransaction>();

        _mockPostgre.Setup(c => c.GetConnection()).ReturnsAsync(connection.Object);
        connection.SetupDapperAsync(c => c.QueryAsync<Product>(It.IsAny<string>(), It.IsAny<object>(), tran.Object,
                It.IsAny<int>(), It.IsAny<CommandType>()))
            .ReturnsAsync(expected);

        //act
        var result = await _repository.GetByCodeAsync("P1");

        //assert
        Assert.IsType<Product>(result);
        Assert.NotNull(result);
    }
}