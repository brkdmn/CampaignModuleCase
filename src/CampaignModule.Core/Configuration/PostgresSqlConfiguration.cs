using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CampaignModule.Core.Configuration;

public class PostgresSqlConfiguration
{
    private readonly IConfiguration _configuration;
    public PostgresSqlConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection GetConnection()
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        var connStr = _configuration["ConnectionStrings:CampaignModuleDB"];
        var connection = new NpgsqlConnection(connStr);
           
        return connection;
    }
}