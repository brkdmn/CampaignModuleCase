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
    
    public async Task<CampaignEntity?> GetCampaignByProductCode(string productCode)
    {
        const string sql = @"SELECT * FROM public.campaign WHERE product_code=@productCode is_active=true AND is_deleted=false;";
        
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
        const string sql = @"SELECT * FROM public.campaign WHERE name=@campaignName is_active=true AND is_deleted=false;";
        
        using var dbConnection = _postgresSqlConfiguration.GetConnection();
        dbConnection.Open();
        
        var result = await dbConnection.QueryAsync<CampaignEntity>(sql, new
        {
            campaignName
        });
        return result.FirstOrDefault();
    }
}