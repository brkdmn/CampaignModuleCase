using System.Data;
using Dapper;
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

    public async Task<IDbConnection> GetConnection()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        var connStr = _configuration["ConnectionStrings:CampaignModuleDB"];
        var connection = new NpgsqlConnection(connStr);
        await connection.OpenAsync();

        return connection;
    }
}