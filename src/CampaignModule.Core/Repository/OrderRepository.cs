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

    public async Task<int> CreateOrder(OrderEntity orderEntity)
    {
        const string sql = @"INSERT INTO public.order
                                (id,product_code,campaign_name,quantity,sale_price,is_active,is_deleted,created_date,updated_date)
                                VALUES (@Id,@ProductCode,@CampaignName,@Quantity,@SalePrice,@IsActive,@IsDeleted,@CreatedDate,@UpdatedDate);";
        
        using var dbConnection = _postgresSqlConfiguration.GetConnection();
        dbConnection.Open();
        
        return await dbConnection
            .ExecuteAsync(sql, orderEntity);
    }
    
    public async Task<TotalQuantity?> GetSalesCountByCampaignNameAndProductCode(string? campaignName, string? productCode)
    {
        const string sql = @"SELECT  SUM(quantity) AS total FROM public.order WHERE 
                        product_code=@productCode AND 
                        campaign_name=@campaignName AND
                        is_active=true AND 
                        is_deleted=false;";
        
        using var dbConnection = _postgresSqlConfiguration.GetConnection();
        dbConnection.Open();
        
        var result = await dbConnection.QueryAsync<TotalQuantity>(sql, new
        {
            productCode,
            campaignName
        });
        return result.FirstOrDefault();
    }
    
    public async Task<TotalQuantity?> GetSalesCountByProductCode(string? productCode)
    {
        const string sql = @"SELECT  SUM(quantity) AS total FROM public.order WHERE 
                        product_code=@productCode AND
                        is_active=true AND 
                        is_deleted=false;";
        
        using var dbConnection = _postgresSqlConfiguration.GetConnection();
        dbConnection.Open();
        
        var result = await dbConnection.QueryAsync<TotalQuantity>(sql, new
        {
            productCode
        });
        return result.FirstOrDefault();
    }
    
    public async Task<TotalPrice?> GetTotalPriceByCampaignNameAndProductCode(string? campaignName, string? productCode)
    {
        const string sql = @"SELECT  SUM(quantity*quantity) AS total FROM public.order WHERE 
                        product_code=@productCode AND 
                        campaign_name=@campaignName AND
                        is_active=true AND 
                        is_deleted=false;";
        
        using var dbConnection = _postgresSqlConfiguration.GetConnection();
        dbConnection.Open();
        
        var result = await dbConnection.QueryAsync<TotalPrice>(sql, new
        {
            productCode,
            campaignName
        });
        return result.FirstOrDefault();
    }
}