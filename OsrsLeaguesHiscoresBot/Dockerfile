FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OsrsLeaguesHiscoresBot/OsrsLeaguesHiscoresBot.csproj", "OsrsLeaguesHiscoresBot/"]
RUN dotnet restore "OsrsLeaguesHiscoresBot/OsrsLeaguesHiscoresBot.csproj"
COPY . .
WORKDIR "/src/OsrsLeaguesHiscoresBot"
RUN dotnet build "OsrsLeaguesHiscoresBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OsrsLeaguesHiscoresBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OsrsLeaguesHiscoresBot.dll"]
