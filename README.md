# Shked

***
## О проекте

Серверная часть приложения для публикации учебных заданий и просмотра расписания.

Используемые технологии: 
* [С# .NET](https://dotnet.microsoft.com/en-us/download), 
[ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet)
* [MongoDB](https://www.mongodb.com/)
* [Amazon S3](https://aws.amazon.com/ru/s3/)
* [Nginx](https://nginx.org/)
* [Docker](https://www.docker.com/), [Docker Compose](https://docs.docker.com/compose/)

## Оглавление

* [Архитектура микросервисов](#архитектура-микросервисов)
* [Инструкция по развертыванию](#инструкция-по-развертыванию)
* [Документация](#документация)
* [Контакты](#контакты)

## Архитектура микросервисов
***
![architecture_scheme.png](images%2Farchitecture_scheme.png)

В проекте используются 5 микросервисов:

* **Auth Service** - предназначен для авторизации пользователей при помощи системы JWT-токенов
* **User Service** - используется для получения и изменения личной информации о пользователе
* **Groups Service** - сервис для управления учебными группами, а также получения расписания от API ВУЗа
* **Task Service** - предназначен для публикации и получения учебных заданий
* **Storage Service API** - надстройка над объектным хранилищем

## Инструкция по развертыванию
Проверьте, что у вас установлен:
* .NET 6.0 SDK -  `dotnet --info`
* Docker - `docker -v`
* Docker Compose - `docker compose version` 

### Процесс развертывания
#### Запуск микросервисов
* Перейти в директорию `./deployment/remote-deployment` и запустить команду `docker compose up --detach`
#### Настройка S3-совместимого хранилища Minio
* Зайти в браузере по адресу `http://localhost:9001/` данные для входа в панель администратора по умолчанию:
``` 
Username: storage-api 
Password: 12345678
```
* Во вкладке Buckets нажимаем `Create Bucket` в правом верхнем углу и обязательно указываем имя bucket'а: `shked-tasks`
* Во вкладке Users нажимаем `Create User` и указываем следующие данные учетной записи:
```
User Name: tasks-service
Password: 12345678
```
`-` В политике доступа необходимы выдать пользователю права `readwrite`

## Документация

`-` Для генерации документов можно использовать скрипт `bash ./docs/gen.sh`

***

* ### [Auth Service](docs/Shked-Authorization.yaml)
* ### [User Service](docs/Shked-UsersService.yaml)
* ### [Groups Service](docs/Shked-GroupsService.yaml)
* ### [Task Service](docs/Shked-TasksService.yaml)
* ### [Storage Service API](docs/Shked-StorageService.yaml)

## Контакты
***
Telegram: [@maermilLov](https://t.me/maermilLov) \
Email: [maks.ermilov.03@mail.ru](maks.ermilov.03@mail.ru)

