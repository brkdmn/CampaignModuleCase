using CampaignModule.Core.Configuration;
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
    
    public async Task<int> CreateCampaign(CampaignEntity campaignEntity)
    {
        const string sql = @"INSERT INTO public.campaign
                                (id,name,product_code,duration,current_duration,price_manipulation_limit,target_sales_count,is_active,is_deleted,created_date,updated_date)
                                VALUES (@Id,@Name,@ProductCode,@Duration,@CurrentDuration,@PriceManipulationLimit,@TargetSalesCount,@IsActive,@IsDeleted,@CreatedDate,@UpdatedDate);";
        
        using var dbConnection = _postgresSqlConfiguration.GetConnection();
        dbConnection.Open();
        
        return await dbConnection
            .ExecuteAsync(sql, campaignEntity);
    }
    
    public async Task<CampaignEntity?> GetCampaignByProductCode(string productCode)
    {
        const string sql = @"SELECT * FROM public.campaign 
         WHERE product_code=@productCode 
           AND is_active=true 
           AND is_deleted=false;";
        
        using var dbConnection = _postgresSqlConfiguration.GetConnection();
        dbConnection.Open();
        
        var result = await dbConnection.QueryAsync<CampaignEntity>(sql, new
        {
            productCode
        });
        return result.FirstOrDefault();
    }
    
    public async Task<CampaignEntity?> GetCampaignByCampaignName(string campaignName)
    {
        const string sql = @"SELECT * FROM public.campaign 
         WHERE name=@campaignName 
           AND is_active=true 
           AND is_deleted=false;";
        
        using var dbConnection = _postgresSqlConfiguration.GetConnection();
        dbConnection.Open();
        
        var result = await dbConnection.QueryAsync<CampaignEntity>(sql, new
        {
            campaignName
        });
        return result.FirstOrDefault();
    }

    public async Task<int> UpdateCampaign(CampaignEntity campaignEntity)
    {
        const string sql = @"UPDATE public.campaign SET
                                id=@Id,
                                name=@Name,
                                product_code=@ProductCode,
                                duration=@Duration,
                                current_duration=@CurrentDuration,
                                price_manipulation_limit=@PriceManipulationLimit,
                                target_sales_count=@TargetSalesCount,
                                is_active=@IsActive,
                                is_deleted=@IsDeleted,
                                updated_date=@UpdatedDate
                                WHERE name=@Name;";
        
        using var dbConnection = _postgresSqlConfiguration.GetConnection();
        dbConnection.Open();
        
        return await dbConnection
            .ExecuteAsync(sql, campaignEntity);
    }
}