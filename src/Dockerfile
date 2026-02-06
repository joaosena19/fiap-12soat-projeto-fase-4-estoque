# Estágio base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Estágio de build 
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["OficinaMecanica/API.csproj", "OficinaMecanica/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "OficinaMecanica/API.csproj"

COPY . .

WORKDIR "/src/OficinaMecanica"
RUN dotnet publish "API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Install the New Relic agent
USER root
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
    && echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
    && wget https://download.newrelic.com/548C16BF.gpg \
    && apt-key add 548C16BF.gpg \
    && apt-get update \
    && apt-get install -y 'newrelic-dotnet-agent' \
    && rm -rf /var/lib/apt/lists/*

# Enable the New Relic agent
ENV CORECLR_ENABLE_PROFILING=1 \
    CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
    CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
    CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
    NEW_RELIC_APP_NAME="Fiap.Fase3.Oficina.API"

# Copia a aplicação
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "API.dll"]