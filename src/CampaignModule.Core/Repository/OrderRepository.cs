using CampaignModule.Core.Configuration;
using CampaignModule.Core.Interfaces.Repository;
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

    public async Task<int> AddAsync(Order order)
    {
        const string sql = @"INSERT INTO public.order
                                (id,product_code,campaign_name,quantity,sale_price,is_active,is_deleted,created_date,updated_date)
                                VALUES (@Id,@ProductCode,@CampaignName,@Quantity,@SalePrice,@IsActive,@IsDeleted,@CreatedDate,@UpdatedDate);";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        return await dbConnection.ExecuteAsync(sql, order);
    }

    public async Task<IEnumerable<Order>?> GetAllAsync()
    {
        const string sql = @"SELECT * FROM public.order WHERE 
                        is_active=true AND 
                        is_deleted=false;";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        return await dbConnection.QueryAsync<Order>(sql);
    }

    public async Task<Order?> GetByCodeAsync(string id)
    {
        const string sql = @"SELECT * FROM public.order 
                            WHERE is_active=true 
                                AND is_deleted=false
                                AND id = @Id;";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        var result = await dbConnection.QueryAsync<Order>(sql, new
        {
            Id = new Guid(id)
        });

        return result.FirstOrDefault();
    }

    public async Task<int> UpdateAsync(Order order)
    {
        const string sql = @"UPDATE public.order
                                SET 
                                    product_code=@ProductCode,
                                    campaign_name=@ProductCode,
                                    quantity=@ProductCode,
                                    sale_price=@ProductCode,
                                    is_active=@ProductCode,
                                    updated_date=NOW() 
                                WHERE id=@Id;";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        return await dbConnection.ExecuteAsync(sql, order);
    }

    public async Task<int> DeleteAsync(string id)
    {
        const string sql = @"UPDATE public.order
                                SET 
                                    is_active=false,
                                    is_deleted=true,
                                    updated_date=NOW() 
                                WHERE id=@Id;";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        return await dbConnection.ExecuteAsync(sql, new { Id = new Guid(id) });
    }

    public async Task<TotalQuantity?> GetSalesCountByCampaignNameAndProductCodeAsync(string? campaignName,
        string? productCode)
    {
        const string sql = @"SELECT  SUM(quantity) AS total FROM public.order WHERE 
                        product_code=@productCode AND 
                        campaign_name=@campaignName AND
                        is_active=true AND 
                        is_deleted=false;";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        var result = await dbConnection.QueryAsync<TotalQuantity>(sql, new
        {
            productCode,
            campaignName
        });

        return result.FirstOrDefault();
    }

    public async Task<TotalQuantity?> GetSalesCountByProductCodeAsync(string? productCode)
    {
        const string sql = @"SELECT  SUM(quantity) AS total FROM public.order WHERE 
                        product_code=@productCode AND
                        is_active=true AND 
                        is_deleted=false;";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        var result = await dbConnection.QueryAsync<TotalQuantity>(sql, new
        {
            productCode
        });

        return result.FirstOrDefault();
    }

    public async Task<TotalPrice?> GetTotalPriceByCampaignNameAndProductCodeAsync(string? campaignName,
        string? productCode)
    {
        const string sql = @"SELECT  SUM(quantity*quantity) AS total FROM public.order WHERE 
                        product_code=@productCode AND 
                        campaign_name=@campaignName AND
                        is_active=true AND 
                        is_deleted=false;";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        var result = await dbConnection.QueryAsync<TotalPrice>(sql, new
        {
            productCode,
            campaignName
        });

        return result.FirstOrDefault();
    }
}