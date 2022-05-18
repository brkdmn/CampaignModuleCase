# CampaignModuleCase

## Proje Bilgileri
Proje C# ile .NET 6 framework kullanılarak geliştirildi.
Database olarak ise postgreSQL kullanılmakta.
ORM aracı olarak bir micro orm tool'u olan Dapper kullanıldı. 

## Kurulum
Proje dizininde sırası ile aşağıdaki komutları kullanarak 
hem uygulamayı hem de kullanılan database'i ayağa kaldırabilirsiniz.

```sh
docker-compose build
docker-compose up
```

Uygulamalar aşağıdaki adreslerden erişilebilir olacaktır.
- CampaignModule.Api -> localhost:5001
- CampaignModule.Api Swagger -> localhost:5001/swagger
- postgreSql -> localhost:5432

## Konfigürasyon

Database ConnectionString:
```sh
User ID=dbuser;Password=1q2w3e;Server=localhost;Port=5432;Database=postgres;Pooling=true;
```

docker-compose.yml dosyası içerisinden aşağıdaki kısımda env değeri kullanılacak ortama göre değiştirilebilir:

```sh
ASPNETCORE_ENVIRONMENT
```
Geçerli env değerleri:
- Development
- Staging
- Test