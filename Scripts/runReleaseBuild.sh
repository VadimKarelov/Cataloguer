#!/bin/sh

cd ../Cataloguer.Server
dotnet build -c Release
dotnet run ./Cataloguer.Server.csproj