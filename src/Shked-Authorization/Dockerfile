FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Shked-Authorization/Shked-Authorization.csproj", "Shked-Authorization/"]
RUN dotnet restore "Shked-Authorization/Shked-Authorization.csproj"
COPY . .
WORKDIR "/src/Shked-Authorization"
RUN dotnet build "Shked-Authorization.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Shked-Authorization.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shked-Authorization.dll"]
