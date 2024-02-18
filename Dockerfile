FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
#EXPOSE 80
#EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Debug
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_USE_POLLING_FILE_WATCHER=true  
#ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_URLS=http://+:9595
EXPOSE 9595

# Install Node.js
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - \
    && apt-get install -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY ["Cataloguer.Server/Cataloguer.Server.csproj", "Cataloguer.Server/"]
COPY ["Cataloguer.Server/Scripts", "Cataloguer.Server/"]
COPY ["Cataloguer.Database/Cataloguer.Database.csproj", "Cataloguer.Database/"]
COPY ["Cataloguer.Common/Cataloguer.Common.csproj", "Cataloguer.Common/"]
RUN dotnet restore "Cataloguer.Server/Cataloguer.Server.csproj"
COPY . .
WORKDIR "/src/Cataloguer.Server"
RUN dotnet build "Cataloguer.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "Cataloguer.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Cataloguer.Server.dll", "--urls", "http://127.0.0.1:9595", "--server.urls", "http://+:80;https://+:443"]
ENTRYPOINT ["dotnet", "Cataloguer.Server.dll", "--urls", "http://127.0.0.1:9595", "--server.urls", "http://+:9595;"]
