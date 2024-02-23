# Образ для сборки asp .net приложения
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Копирование и восстановление зависимостей asp .net приложения
COPY Cataloguer.Common/*.csproj ./Cataloguer.Common/
COPY Cataloguer.Database/*.csproj ./Cataloguer.Database/
COPY Cataloguer.Server/*.csproj ./Cataloguer.Server/
RUN rm -rf frontend-app/node_modules
COPY frontend-app/* ./frontend-app/

WORKDIR /app/Cataloguer.Server
RUN dotnet restore

# Копирование исходного кода asp .net приложения
COPY Cataloguer.Common/. ../Cataloguer.Common/
COPY Cataloguer.Database/. ../Cataloguer.Database/
COPY Cataloguer.Server/. .
RUN rm -rf frontend-app/node_modules
COPY frontend-app/. ../frontend-app/

RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - \
    && apt-get install -y \
        nodejs
#        nodejs \
#    && rm -rf /var/lib/apt/lists/*

#WORKDIR /app/Cataloguer.Database
#RUN dotnet publish -c Release -o out

WORKDIR /app/Cataloguer.Server
RUN dotnet publish -c Release -o out

# Образ для запуска asp .net приложения
FROM mcr.microsoft.com/dotnet/aspnet:7.0 
WORKDIR /app
#COPY --from=build /app/Cataloguer.Database/out ./
COPY --from=build /app/Cataloguer.Server/out ./

# Открытие порта 9595 для asp .net приложения
EXPOSE 9595
ENTRYPOINT ["dotnet", "Cataloguer.Server.dll"]
