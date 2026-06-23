FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Jartisan.CLI/Jartisan.CLI.csproj", "Jartisan.CLI/"]
RUN dotnet restore "Jartisan.CLI/Jartisan.CLI.csproj"
COPY . .
WORKDIR "/src/Jartisan.CLI"
RUN dotnet build "./Jartisan.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Jartisan.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Jartisan.CLI.dll"]
