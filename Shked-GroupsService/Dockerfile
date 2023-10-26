FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Shked-GroupsService/Shked-GroupsService.csproj", "Shked-GroupsService/"]
RUN dotnet restore "Shked-GroupsService/Shked-GroupsService.csproj"
COPY . .
WORKDIR "/src/Shked-GroupsService"
RUN dotnet build "Shked-GroupsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Shked-GroupsService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shked-GroupsService.dll"]
