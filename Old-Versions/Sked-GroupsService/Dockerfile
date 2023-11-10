FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sked-GroupsService/Sked-GroupsService.csproj", "Sked-GroupsService/"]
RUN dotnet restore "Sked-GroupsService/Sked-GroupsService.csproj"
COPY . .
WORKDIR "/src/Sked-GroupsService"
RUN dotnet build "Sked-GroupsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sked-GroupsService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sked-GroupsService.dll"]
