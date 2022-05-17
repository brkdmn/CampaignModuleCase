using CampaignModule.Core.Configuration;
using CampaignModule.Domain.Entity;
using Dapper;

namespace CampaignModule.Core.Repository;

public class ProductRepository : IProductRepository
{
    private readonly PostgresSqlConfiguration _postgresSqlConfiguration;
    public ProductRepository(PostgresSqlConfiguration postgresSqlConfiguration)
    {
        _postgresSqlConfiguration = postgresSqlConfiguration;
    }
    
    public async Task<int> CreateProduct(ProductEntity productEntity)
    {
        const string sql = @"INSERT INTO public.product
                                (id,product_code,price,stock,is_active,is_deleted,created_date,updated_date)
                                VALUES (@Id,@ProductCode,@Price,@Stock,@IsActive,@IsDeleted,@CreatedDate,@UpdatedDate);";
        
        using var dbConnection = _postgresSqlConfiguration.GetConnection();
        dbConnection.Open();
        
        return await dbConnection
            .ExecuteAsync(sql, productEntity);
    }

    public async Task<ProductEntity?> GetProduct(string productCode)
    {
        const string sql = @"SELECT * FROM public.product WHERE is_active=true AND is_deleted=false;";
        
        using var dbConnection = _postgresSqlConfiguration.GetConnection();
        dbConnection.Open();
        
        var result = await dbConnection.QueryAsync<ProductEntity>(sql);
        return result.FirstOrDefault();
    }
}