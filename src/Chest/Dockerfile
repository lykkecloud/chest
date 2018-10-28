FROM microsoft/dotnet:2.1.0-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1.300-sdk AS build
WORKDIR /src
COPY . ./
WORKDIR /src/Chest
RUN dotnet build -c Release -r linux-x64 -o /app

FROM build AS publish
RUN dotnet publish -c Release -r linux-x64 -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Chest.dll"]