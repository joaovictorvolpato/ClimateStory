
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build


WORKDIR /home/ubuntu/app
COPY . /home/ubuntu/app


RUN dotnet restore


RUN dotnet publish -c Release -o /out


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime


WORKDIR /home/ubuntu/app



COPY --from=build /out .


EXPOSE 80


