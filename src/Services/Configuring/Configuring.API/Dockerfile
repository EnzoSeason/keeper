FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/Configuring/Configuring.API/Configuring.API.csproj", "Services/Configuring/Configuring.API/"]
RUN dotnet restore "Services/Configuring/Configuring.API/Configuring.API.csproj"

COPY . .
WORKDIR "/src/Services/Configuring/Configuring.API"
RUN dotnet build "Configuring.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Configuring.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Configuring.API.dll"]
