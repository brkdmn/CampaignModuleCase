using CampaignModule.Api.Middleware;
using CampaignModule.Core.Configuration;
using CampaignModule.Core.Repository;
using CampaignModule.Core.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<PostgresSqlConfiguration>();
builder.Services.AddScoped<IProductRepository,ProductRepository>();
builder.Services.AddScoped<IOrderRepository,OrderRepository>();
builder.Services.AddScoped<ICampaignRepository,CampaignRepository>();
builder.Services.AddScoped<IProductService,ProductService>();
builder.Services.AddScoped<IOrderService,OrderService>();
builder.Services.AddScoped<ICampaignService,CampaignService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

app.Run();