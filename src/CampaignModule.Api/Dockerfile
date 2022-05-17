FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/CampaignModule.Api/CampaignModule.Api.csproj", "src/CampaignModule.Api/"]
RUN dotnet restore "src/CampaignModule.Api/CampaignModule.Api.csproj"
COPY . .
WORKDIR "/src/src/CampaignModule.Api"
RUN dotnet build "CampaignModule.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CampaignModule.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CampaignModule.Api.dll"]