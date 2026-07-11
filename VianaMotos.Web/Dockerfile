FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["VianaMotos.Web.csproj", "./"]

RUN dotnet restore "VianaMotos.Web.csproj"

COPY . .

RUN dotnet publish "VianaMotos.Web.csproj" \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

FROM base AS final

WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/wwwroot/uploads/motos \
    && chown -R app:app /app/wwwroot/uploads

USER app

ENTRYPOINT ["dotnet", "VianaMotos.Web.dll"]