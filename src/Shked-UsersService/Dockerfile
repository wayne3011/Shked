FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Shked-UsersService/Shked-UsersService.csproj", "Shked-UsersService/"]
COPY ["Shked-User-DAL/Shked-User-DAL.csproj", "Shked-User-DAL/"]
RUN dotnet restore "Shked-UsersService/Shked-UsersService.csproj"
COPY . .
WORKDIR "/src/Shked-UsersService"
RUN dotnet build "Shked-UsersService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Shked-UsersService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shked-UsersService.dll"]
