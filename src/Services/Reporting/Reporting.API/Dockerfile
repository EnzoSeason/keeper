﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/Reporting/Reporting.API/Reporting.API.csproj", "Services/Reporting/Reporting.API/"]
RUN dotnet restore "Services/Reporting/Reporting.API/Reporting.API.csproj"

COPY . .
WORKDIR "/src/Services/Reporting/Reporting.API"
RUN dotnet build "Reporting.API.csproj" -c Release -o /app/build

FROM build AS unittest
WORKDIR "/src/Services/Reporting/Reporting.UTest"

FROM build AS integrationtest
WORKDIR "/src/Services/Reporting/Reporting.ITest"

FROM build AS publish
RUN dotnet publish "Reporting.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reporting.API.dll"]
