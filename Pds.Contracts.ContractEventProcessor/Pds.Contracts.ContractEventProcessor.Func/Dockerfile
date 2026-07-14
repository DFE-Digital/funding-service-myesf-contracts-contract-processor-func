# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/azure-functions/dotnet:3.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

COPY --from=mcr.microsoft.com/dotnet/sdk:3.1 /usr/share/dotnet/sdk /usr/share/dotnet/sdk
COPY --from=mcr.microsoft.com/dotnet/sdk:3.1 /usr/share/dotnet/shared /usr/share/dotnet/shared
COPY --from=mcr.microsoft.com/dotnet/sdk:3.1 /usr/share/dotnet/host /usr/share/dotnet/host

# List SDKs
RUN dotnet --list-sdks

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Pds.Contracts.ContractEventProcessor.Func/Pds.Contracts.ContractEventProcessor.Func.csproj", "Pds.Contracts.ContractEventProcessor.Func/"]
COPY ["Pds.Contracts.ContractEventProcessor.Services/Pds.Contracts.ContractEventProcessor.Services.csproj", "Pds.Contracts.ContractEventProcessor.Services/"]

ARG PDS_FEED_URL="https://sfa-gov-uk.pkgs.visualstudio.com/9fd46a3f-eada-41dc-86e9-36332aeb8ef1/_packaging/PDS/nuget/v3/index.json"
ARG PDS_FEED_USERNAME="localuser"
ARG PDS_FEED_PAT

RUN dotnet nuget add source ${PDS_FEED_URL} \
	--name "Private Source" \
	--username ${PDS_FEED_USERNAME} \
	--password ${PDS_FEED_PAT} \
	--store-password-in-clear-text

RUN dotnet restore "./Pds.Contracts.ContractEventProcessor.Func/Pds.Contracts.ContractEventProcessor.Func.csproj"
COPY . .
WORKDIR "/src/Pds.Contracts.ContractEventProcessor.Func"
RUN dotnet build "./Pds.Contracts.ContractEventProcessor.Func.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Pds.Contracts.ContractEventProcessor.Func.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true