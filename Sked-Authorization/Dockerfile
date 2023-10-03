FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sked-Authorization/Sked-Authorization.csproj", "Sked-Authorization/"]
RUN dotnet restore "Sked-Authorization/Sked-Authorization.csproj"
COPY . .
WORKDIR "/src/Sked-Authorization"
RUN dotnet build "Sked-Authorization.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sked-Authorization.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sked-Authorization.dll"]
