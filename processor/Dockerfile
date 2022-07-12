# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY *.sln ./
COPY ./Kadmium-Dmx-Processor/*.csproj ./Kadmium-Dmx-Processor/

RUN dotnet restore

# Copy everything else and build
COPY ./Kadmium-Dmx-Processor/ ./Kadmium-Dmx-Processor/
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/dist .
ENTRYPOINT ["dotnet", "Kadmium-Dmx-Processor.dll"]