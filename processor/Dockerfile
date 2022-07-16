# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app/Kadmium-Dmx

COPY ./processor/*.sln ./
COPY ./processor/Kadmium-Dmx-Processor/*.csproj ./Kadmium-Dmx-Processor/
COPY ./processor/Kadmium-Dmx-Processor.Test/*.csproj ./Kadmium-Dmx-Processor.Test/
COPY ./shared/*.csproj ../shared/

RUN dotnet restore

# Copy everything else and build
COPY ./processor/Kadmium-Dmx-Processor/ ./Kadmium-Dmx-Processor/
COPY ./processor/Kadmium-Dmx-Processor.Test/ ./Kadmium-Dmx-Processor.Test/
COPY ./shared/ ../shared/
RUN dotnet publish -c Release -o dist

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/Kadmium-Dmx/dist .
ENTRYPOINT ["dotnet", "Kadmium-Dmx-Processor.dll"]