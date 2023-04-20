FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["final-backend/final-backend.csproj", "final-backend/"]
RUN dotnet restore "final-backend/final-backend.csproj"
COPY . .
WORKDIR "/src/final-backend"
RUN dotnet build "final-backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "final-backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "final-backend.dll"]
