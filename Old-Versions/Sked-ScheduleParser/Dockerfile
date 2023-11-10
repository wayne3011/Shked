FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sked-ScheduleParser/Sked-ScheduleParser.csproj", "Sked-ScheduleParser/"]
RUN dotnet restore "Sked-ScheduleParser/Sked-ScheduleParser.csproj"
COPY . .
WORKDIR "/src/Sked-ScheduleParser"
RUN dotnet build "Sked-ScheduleParser.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sked-ScheduleParser.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sked-ScheduleParser.dll"]
