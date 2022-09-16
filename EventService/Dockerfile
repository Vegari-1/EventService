FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
ARG github_username
ARG github_pat
WORKDIR /src
COPY EventService/EventService.csproj EventService/
COPY config/NuGet.Config .
RUN dotnet restore EventService/EventService.csproj
COPY . .
WORKDIR /src/EventService
RUN dotnet build EventService.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish EventService.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "EventService.dll"]