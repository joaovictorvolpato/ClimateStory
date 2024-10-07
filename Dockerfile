
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build


WORKDIR /app


RUN apt-get update && apt-get install -y git


RUN git clone https://github.com/joaovictorvolpato/ClimateStory.git .


RUN dotnet restore


RUN dotnet publish -c Release -o /out


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime


WORKDIR /app


COPY --from=build /out .


EXPOSE 80

