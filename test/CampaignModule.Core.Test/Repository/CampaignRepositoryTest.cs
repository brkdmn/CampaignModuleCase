using System.Data;
using System.Data.Common;
using CampaignModule.Core.Configuration;
using CampaignModule.Core.Repository;
using CampaignModule.Domain.Entity;
using Dapper;
using Moq;
using Moq.Dapper;

namespace CampaignModule.Core.Test.Repository;

public class CampaignRepositoryTest
{
    private readonly Mock<IPostgresSqlConfiguration> _mockPostgre;
    private readonly CampaignRepository _repository;

    public CampaignRepositoryTest()
    {
        _mockPostgre = new Mock<IPostgresSqlConfiguration>();
        _repository = new CampaignRepository(_mockPostgre.Object);
    }

    [Fact]
    public async Task GetCampaignByProductCodeAsync_ReturnsCampaign()
    {
        //arrange
        var connection = new Mock<IDbConnection>();
        var mockCampaign = Mock.Of<Campaign>();
        var expected = new[] { mockCampaign };
        var tran = new Mock<DbTransaction>();

        _mockPostgre.Setup(c => c.GetConnection()).ReturnsAsync(connection.Object);
        connection.SetupDapperAsync(c => c.QueryAsync<Campaign>(It.IsAny<string>(), It.IsAny<object>(), tran.Object,
                It.IsAny<int>(), It.IsAny<CommandType>()))
            .ReturnsAsync(expected);

        //act
        var result = await _repository.GetCampaignByProductCodeAsync("P1");

        //assert
        Assert.IsType<Campaign>(result);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsCampaigns()
    {
        //arrange
        var connection = new Mock<IDbConnection>();
        var mockCampaign = Mock.Of<Campaign>();
        var expected = new[] { mockCampaign };
        var tran = new Mock<DbTransaction>();

        _mockPostgre.Setup(c => c.GetConnection()).ReturnsAsync(connection.Object);
        connection.SetupDapperAsync(c => c.QueryAsync<Campaign>(It.IsAny<string>(), It.IsAny<object>(), tran.Object,
                It.IsAny<int>(), It.IsAny<CommandType>()))
            .ReturnsAsync(expected);

        //act
        var result = await _repository.GetAllAsync();

        //assert
        Assert.IsAssignableFrom<IEnumerable<Campaign>>(result);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByCodeAsync_ReturnsCampaigns()
    {
        //arrange
        var connection = new Mock<IDbConnection>();
        var mockCampaign = Mock.Of<Campaign>();
        var expected = new[] { mockCampaign };
        var tran = new Mock<DbTransaction>();

        _mockPostgre.Setup(c => c.GetConnection()).ReturnsAsync(connection.Object);
        connection.SetupDapperAsync(c => c.QueryAsync<Campaign>(It.IsAny<string>(), It.IsAny<object>(), tran.Object,
                It.IsAny<int>(), It.IsAny<CommandType>()))
            .ReturnsAsync(expected);

        //act
        var result = await _repository.GetByCodeAsync("C1");

        //assert
        Assert.IsType<Campaign>(result);
        Assert.NotNull(result);
    }
}