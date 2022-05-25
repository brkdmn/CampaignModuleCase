using CampaignModule.Core.Configuration;
using CampaignModule.Core.Interfaces.Repository;
using CampaignModule.Domain.Entity;
using Dapper;

namespace CampaignModule.Core.Repository;

public class ProductRepository : IProductRepository
{
    private readonly IPostgresSqlConfiguration _postgresSqlConfiguration;

    public ProductRepository(IPostgresSqlConfiguration postgresSqlConfiguration)
    {
        _postgresSqlConfiguration = postgresSqlConfiguration;
    }

    public async Task<int> AddAsync(Product product)
    {
        const string sql = @"INSERT INTO public.product
                                (id,product_code,price,stock,is_active,is_deleted,created_date,updated_date)
                                VALUES (@Id,@ProductCode,@Price,@Stock,@IsActive,@IsDeleted,@CreatedDate,@UpdatedDate);";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        return await dbConnection.ExecuteAsync(sql, product);
    }

    public async Task<IEnumerable<Product>?> GetAllAsync()
    {
        const string sql = @"SELECT * FROM public.product 
                                WHERE is_active=true 
                                AND is_deleted=false;";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        return await dbConnection.QueryAsync<Product>(sql);
    }

    public async Task<Product?> GetByCodeAsync(string productCode)
    {
        const string sql = @"SELECT * FROM public.product 
                                WHERE is_active=true 
                                AND is_deleted=false 
                                AND product_code=@productCode;";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        var result = await dbConnection.QueryAsync<Product>(sql, new
        {
            productCode
        });

        return result.FirstOrDefault();
    }

    public async Task<int> UpdateAsync(Product product)
    {
        const string sql = @"UPDATE public.product
                                SET 
                                    price=@Price,
                                    stock=@Stock,
                                    is_active=@IsActive,
                                    updated_date=@UpdatedDate
                                 WHERE
                                     product_code=@ProductCode";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        return await dbConnection.ExecuteAsync(sql, product);
    }

    public async Task<int> DeleteAsync(string code)
    {
        const string sql = @"UPDATE public.product
                                SET 
                                    is_active=false,
                                    is_deleted=true
                                    updated_date=NOW()
                                 WHERE
                                     product_code=@ProductCode";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        return await dbConnection.ExecuteAsync(sql, new
        {
            code
        });
    }
}