# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY ./sources/RtpMidi/ *.sln ./sources/RtpMidi/
COPY ./sources/RtpMidi/RtpMidiSource/*.csproj ./sources/RtpMidi/RtpMidiSource/
COPY ./sources/Shared/*.csproj ./Shared/

WORKDIR ./sources/Shared/

RUN dotnet restore

# Copy everything else and build
COPY ./sources/RtpMidi/RtpMidiSource/ ./sources/RtpMidi/RtpMidiSource/
RUN dotnet publish -c Release -o dist

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

EXPOSE 1900/udp
EXPOSE 5023/udp
EXPOSE 5024/udp
EXPOSE 5350/udp
EXPOSE 5351/udp
EXPOSE 5353/udp

WORKDIR /app
COPY --from=build-env /app/dist .
ENTRYPOINT ["dotnet", "RtpMidiSource.dll"]