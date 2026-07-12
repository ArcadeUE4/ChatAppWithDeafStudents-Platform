
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src


COPY ["ChatAppWithDeafStudents.API.csproj", "ChatAppWithDeafStudents/"]
COPY ["ChatAppWithDeafStudents.Client/ChatAppWithDeafStudents.Client.csproj", "ChatAppWithDeafStudents.Client/"]


WORKDIR /src/ChatAppWithDeafStudents
RUN dotnet restore "ChatAppWithDeafStudents.API.csproj"


WORKDIR /src
COPY . .


WORKDIR /src/ChatAppWithDeafStudents
RUN dotnet build "ChatAppWithDeafStudents.API.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "ChatAppWithDeafStudents.API.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app


RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*


COPY --from=publish /app/publish .


EXPOSE 8080


HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1


ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080


ENTRYPOINT ["dotnet", "ChatAppWithDeafStudents.API.dll"]
