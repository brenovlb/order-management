FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY src/OrderManagement.Domain/OrderManagement.Domain.csproj src/OrderManagement.Domain/
COPY src/OrderManagement.Application/OrderManagement.Application.csproj src/OrderManagement.Application/
COPY src/OrderManagement.Infrastructure/OrderManagement.Infrastructure.csproj src/OrderManagement.Infrastructure/
COPY src/OrderManagement.Api/OrderManagement.Api.csproj src/OrderManagement.Api/

RUN dotnet restore src/OrderManagement.Api/OrderManagement.Api.csproj

COPY src/ ./src/

RUN dotnet publish \
    src/OrderManagement.Api/OrderManagement.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "OrderManagement.Api.dll"]