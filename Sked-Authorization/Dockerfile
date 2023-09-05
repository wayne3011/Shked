FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sked-Authoriztion/Sked-Authoriztion.csproj", "Sked-Authoriztion/"]
RUN dotnet restore "Sked-Authoriztion/Sked-Authoriztion.csproj"
COPY . .
WORKDIR "/src/Sked-Authoriztion"
RUN dotnet build "Sked-Authoriztion.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sked-Authoriztion.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sked-Authoriztion.dll"]
