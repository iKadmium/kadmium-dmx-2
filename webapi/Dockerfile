# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app/webapi

COPY ./webapi/*.sln ./
COPY ./webapi/Webapi/*.csproj ./Webapi/
COPY ./webapi/Webapi.Test/*.csproj ./Webapi.Test/
COPY ./shared/*.csproj ../shared/

RUN dotnet restore

# Copy everything else and build
COPY ./webapi/Webapi/ ./Webapi/
COPY ./webapi/Webapi.Test/ ./Webapi.Test/
COPY ./shared/ ../shared/
RUN dotnet publish -c Release -o dist

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/webapi/dist .

EXPOSE 80

ENTRYPOINT ["dotnet", "Webapi.dll"]