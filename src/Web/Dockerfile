FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/Web/Web.csproj", "src/Web/"]
RUN dotnet restore "src/Web/Web.csproj"
COPY . .
WORKDIR "/src/src/Web"
RUN dotnet build "Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Web.dll"]
