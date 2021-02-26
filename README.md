# XkgWiki

## Лицензия

Подробности в [LICENSE](/LICENSE).

## Docker

Для работы образа через https необходимо выполнить комманды ниже. Пожалуйста, разберитесь что они делают. Вы можете самостоятельно отключить https, создав `docker-compose.override.yml`. Сайт по умолчанию доступен по адресу https://localhost:8581/ (Docker) или https://localhost:5001/.

 - PowerShell

```powershell
dotnet dev-certs https --clean
dotnet dev-certs https --trust -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p SECRETPASSWORD
```

 - Bash

```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust -ep $HOME/.aspnet/https/aspnetapp.pfx -p SECRETPASSWORD
```
