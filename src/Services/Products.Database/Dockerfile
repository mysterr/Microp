FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/Services/Products.Database/Products.Database.csproj", "src/Services/Products.Database/"]
RUN dotnet restore "src/Services/Products.Database/Products.Database.csproj"
COPY . .
WORKDIR "/src/src/Services/Products.Database"
RUN dotnet build "Products.Database.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Products.Database.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Products.Database.dll"]