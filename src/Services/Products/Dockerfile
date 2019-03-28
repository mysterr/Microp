FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/services/products/Products.csproj", "src/services/products/"]
RUN dotnet restore "src/services/products/Products.csproj"
COPY . .
WORKDIR "/src/src/services/products"
RUN dotnet build "Products.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Products.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Products.dll"]