FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/Services/Products.Queue/Products.Queue.csproj", "src/Services/Products.Queue/"]
RUN dotnet restore "src/Services/Products.Queue/Products.Queue.csproj"
COPY . .
WORKDIR "/src/src/Services/Products.Queue"
RUN dotnet build "Products.Queue.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Products.Queue.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Products.Queue.dll"]