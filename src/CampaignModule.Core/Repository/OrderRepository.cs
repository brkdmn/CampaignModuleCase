using CampaignModule.Core.Configuration;
using CampaignModule.Domain.Entity;
using Dapper;

namespace CampaignModule.Core.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly PostgresSqlConfiguration _postgresSqlConfiguration;
    public OrderRepository(PostgresSqlConfiguration postgresSqlConfiguration)
    {
        _postgresSqlConfiguration = postgresSqlConfiguration;
    }

    public async Task<int> GetSalesCountByCampaignNameAndProductCode(string? campaignName, string? productCode)
    {
        const string sql = @"SELECT  SUM(quantity) AS total FROM public.order WHERE 
                        product_code=@productCode AND 
                        campaign_name=@campaignName AND
                        is_active=true AND 
                        is_deleted=false;";
        
        using var dbConnection = _postgresSqlConfiguration.GetConnection();
        dbConnection.Open();
        
        var result = await dbConnection.QueryAsync<dynamic>(sql, new
        {
            productCode,
            campaignName
        });
        return (int)result.FirstOrDefault()["total"];
    }
    
    public async Task<decimal> GetTotalPriceByCampaignNameAndProductCode(string? campaignName, string? productCode)
    {
        const string sql = @"SELECT  SUM(quantity*quantity) AS total FROM public.order WHERE 
                        product_code=@productCode AND 
                        campaign_name=@campaignName AND
                        is_active=true AND 
                        is_deleted=false;";
        
        using var dbConnection = _postgresSqlConfiguration.GetConnection();
        dbConnection.Open();
        
        var result = await dbConnection.QueryAsync<dynamic>(sql, new
        {
            productCode,
            campaignName
        });
        return (decimal)result.FirstOrDefault()["total"];
    }
}