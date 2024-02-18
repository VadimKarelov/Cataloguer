#!/bin/sh

cd ../Cataloguer.Server
dotnet build -c Debug
dotnet run ./Cataloguer.Server.csproj