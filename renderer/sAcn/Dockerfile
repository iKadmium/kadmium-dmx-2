# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY ./renderer/sAcn/*.sln ./
COPY ./renderer/sAcn/SacnRenderer/*.csproj ./SacnRenderer/
COPY ./renderer/sAcn/SacnRenderer.Test/*.csproj ./SacnRenderer.Test/

RUN dotnet restore

# Copy everything else and build
COPY ./renderer/sAcn/SacnRenderer/ ./SacnRenderer/
COPY ./renderer/sAcn/SacnRenderer.Test/ ./SacnRenderer.Test/
RUN dotnet publish -c Release -o dist

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/dist .
ENTRYPOINT ["dotnet", "SacnRenderer.dll"]