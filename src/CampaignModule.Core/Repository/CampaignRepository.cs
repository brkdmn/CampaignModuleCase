using CampaignModule.Core.Configuration;
using CampaignModule.Core.Interfaces.Repository;
using CampaignModule.Domain.Entity;
using Dapper;

namespace CampaignModule.Core.Repository;

public class CampaignRepository : ICampaignRepository
{
    private readonly PostgresSqlConfiguration _postgresSqlConfiguration;

    public CampaignRepository(PostgresSqlConfiguration postgresSqlConfiguration)
    {
        _postgresSqlConfiguration = postgresSqlConfiguration;
    }

    public async Task<Campaign?> GetCampaignByProductCodeAsync(string productCode)
    {
        const string sql = @"SELECT * FROM public.campaign 
         WHERE product_code=@productCode 
           AND is_active=true 
           AND is_deleted=false;";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        var result = await dbConnection.QueryAsync<Campaign>(sql, new
        {
            productCode
        });

        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Campaign>?> GetAllAsync()
    {
        const string sql = @"SELECT * FROM public.campaign 
                                WHERE is_active=true 
                                  AND is_deleted=false;";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        return await dbConnection.QueryAsync<Campaign>(sql);
    }

    public async Task<int> AddAsync(Campaign campaign)
    {
        const string sql = @"INSERT INTO public.campaign
                                (id,name,product_code,duration,current_duration,price_manipulation_limit,target_sales_count,is_active,is_deleted,created_date,updated_date)
                                VALUES (@Id,@Name,@ProductCode,@Duration,@CurrentDuration,@PriceManipulationLimit,@TargetSalesCount,@IsActive,@IsDeleted,@CreatedDate,@UpdatedDate);";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        return await dbConnection.ExecuteAsync(sql, campaign);
    }

    public async Task<Campaign?> GetByCodeAsync(string campaignName)
    {
        const string sql = @"SELECT * FROM public.campaign 
         WHERE name=@campaignName 
           AND is_active=true 
           AND is_deleted=false;";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        var result = await dbConnection.QueryAsync<Campaign>(sql, new
        {
            campaignName
        });

        return result.FirstOrDefault();
    }

    public async Task<int> UpdateAsync(Campaign campaign)
    {
        const string sql = @"UPDATE public.campaign SET
                                name=@Name,
                                product_code=@ProductCode,
                                duration=@Duration,
                                current_duration=@CurrentDuration,
                                price_manipulation_limit=@PriceManipulationLimit,
                                target_sales_count=@TargetSalesCount,
                                is_active=@IsActive,
                                updated_date=@UpdatedDate
                                WHERE name=@Name;";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        return await dbConnection.ExecuteAsync(sql, campaign);
    }

    public async Task<int> DeleteAsync(string name)
    {
        const string sql = @"UPDATE public.campaign SET
                                is_active = false,
                                is_deleted=true,
                                updated_date=NOW()
                                WHERE name=@Name;";

        using var dbConnection = await _postgresSqlConfiguration.GetConnection();
        return await dbConnection.ExecuteAsync(sql, new { Name = name });
    }
}