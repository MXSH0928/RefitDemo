#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["DemoApiFive/DemoApiFive.csproj", "DemoApiFive/"]
RUN dotnet restore "DemoApiFive/DemoApiFive.csproj"
COPY . .
WORKDIR "/src/DemoApiFive"
RUN dotnet build "DemoApiFive.csproj" -c Release -o /app/build

# In build stage
# Install desired .NET CLI diagnostics tools
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS tools-install
RUN dotnet tool install --tool-path /tools dotnet-trace
RUN dotnet tool install --tool-path /tools dotnet-counters
RUN dotnet tool install --tool-path /tools dotnet-dump
RUN dotnet tool install --tool-path /tools dotnet-gcdump

# In final stage
# Copy diagnostics tools
WORKDIR /tools
COPY --from=build /tools .

FROM build AS publish
RUN dotnet publish "DemoApiFive.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoApiFive.dll"]