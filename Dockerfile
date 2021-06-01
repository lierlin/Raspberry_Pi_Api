#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Pi_Api/Pi_Api.csproj", "Pi_Api/"]
COPY ["Model/Model.csproj", "Model/"]
COPY ["IServices/IServices.csproj", "IServices/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["CompanyManageIRepository/IRepository.csproj", "CompanyManageIRepository/"]
COPY ["CompanyManageRepository/Repository.csproj", "CompanyManageRepository/"]
RUN dotnet restore "./Pi_Api/Pi_Api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Pi_Api/Pi_Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pi_Api/Pi_Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pi_Api.dll"]