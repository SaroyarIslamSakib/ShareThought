# ========== BUILD STAGE ==========
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

RUN apt update && apt install -y nodejs

COPY ["DevSkill.Blog.Web/DevSkill.Blog.Web.csproj", "DevSkill.Blog.Web/"]
COPY ["DevSkill.Blog.Domain/DevSkill.Blog.Domain.csproj", "DevSkill.Blog.Domain/"]
COPY ["DevSkill.Blog.Application/DevSkill.Blog.Application.csproj", "DevSkill.Blog.Application/"]
COPY ["DevSkill.Blog.Infrastructure/DevSkill.Blog.Infrastructure.csproj", "DevSkill.Blog.Infrastructure/"]

RUN dotnet restore "DevSkill.Blog.Web/DevSkill.Blog.Web.csproj"

COPY . .

WORKDIR "/src/DevSkill.Blog.Web"
RUN dotnet publish "DevSkill.Blog.Web.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "DevSkill.Blog.Web.dll"]
