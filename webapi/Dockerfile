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


# Build UI
# syntax=docker/dockerfile:1
FROM node:latest AS ui-build-env
WORKDIR /app/

COPY ./webui/package.json ./

RUN npm install

# Copy everything else and build
COPY ./webui ./
RUN npm run build

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/webapi/dist .
COPY --from=ui-build-env /app/build ./htdocs/

EXPOSE 80

ENTRYPOINT ["dotnet", "Webapi.dll"]