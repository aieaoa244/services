FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["EmployeeService.csproj", "./"]
RUN dotnet restore "EmployeeService.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "EmployeeService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmployeeService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmployeeService.dll"]
