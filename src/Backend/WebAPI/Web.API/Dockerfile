
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BaseApp/src/Backend/WebAPI/Web.API/Web.API.csproj", "BaseApp/src/Backend/WebAPI/Web.API/"]
COPY ["BaseApp/src/Backend/Core/Application/Application.csproj", "BaseApp/src/Backend/Core/Application/"]
COPY ["BaseApp/src/Backend/Shared/Shared/Shared.csproj", "BaseApp/src/Backend/Shared/Shared/"]
COPY ["BaseApp/src/Backend/Core/Domain/Domain.csproj", "BaseApp/src/Backend/Core/Domain/"]
COPY ["BaseApp/src/Backend/Infrastructure/Infrastructure/Infrastructure.csproj", "BaseApp/src/Backend/Infrastructure/Infrastructure/"]
COPY ["BaseApp/src/Backend/Infrastructure/Persistence/Persistence.csproj", "BaseApp/src/Backend/Infrastructure/Persistence/"]
RUN dotnet restore "./BaseApp/src/Backend/WebAPI/Web.API/Web.API.csproj"
COPY . .
WORKDIR "/src/BaseApp/src/Backend/WebAPI/Web.API"
RUN dotnet build "./Web.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.API.dll"]