FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Shked-UsersService/Shked-UsersService.csproj", "Shked-UsersService/"]
RUN dotnet restore "Shked-UsersService/Shked-UsersService.csproj"
COPY . .
WORKDIR "/src/Shked-UsersService"
RUN dotnet build "Shked-UsersService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Shked-UsersService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shked-UsersService.dll"]
