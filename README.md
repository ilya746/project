# App.config
### Перед запуском приложения необходимо настроить файл конфигурации App.config.

1)  Для подключения к базе данных указывается строка подключения и размер диска.

## Строка подключения указывается внутри тегов:

```
<connectionStrings> 
<add name="serverName" providerName="Npgsql" connectionString="Server=serverName;Port=portValue;User ID=userName;Password=passwordValue;Database=dbName;" />
</connectionStrings>
```

где
+ serverName - название сервера
+ portValue - значение порта
+ userName - имя пользователя бд
+ passwordValue - пароль пользователя бд
+ dbName - название бд

#### Пример:

```
<connectionStrings>
<add name="localhost" providerName="Npgsql" connectionString="Server=localhost;Port=5432;User ID=admin;Password=12345;Database=database1;" />
</connectionStrings>
```

## Размер диска указывается между тегами
```<appSettings> <add key="serverSize" value="sizeValue" /> </appSettings>```

где sizeValue - размер диска

#### Пример:

```<appSettings> <add key="serverSize" value="3" /> </appSettings>```


2)  Для подключения к аккаунту Google необходимо указать данные пользовательского аккаунта.

Для этого требуется [json файл](https://developers.google.com/sheets/api/quickstart/dotnet). Файл генерируется автоматически после нажатия на кнопку "Enable the Google Sheet API" --> "Create".  
Содержимое сгенерированного файла необходимо вставить между тегами

```
<appSettings><add key="credentials" value='userCredential'/></appSettings>
``` 

где userCredential - содержимое сгенерированного файла

#### Пример:

```
<appSettings>
<add key="credentials" value='{"installed":{"client_id":"766348-p2n1fk5q04qak5tgschsvkn7a2c.apps.googleusercontent.com","project_id":"quickstart-1585666415280","auth_uri":"https://accounts.google.com/o/oauth2/auth","token_uri":"https://oauth2.googleapis.com/token","auth_provider_x509_cert_url":"https://www.googleapis.com/oauth2/v1/certs","client_secret":"M5wm08jwi3BX3ZdAM","redirect_uris":["urn:ietf:wg:oauth:2.0:oob","http://localhost"]}}'/>
</appSettings>
``` 

3) Для подключения к google tables необходимо указать ID таблицы. Он берется из ссылки на документ таблицы:

> https://docs.google.com/spreadsheets/d/tableIdValue/edit#gid=0
 
 где tableIdValue - ID таблицы
 
указывается между тегами

```
<appSettings>
<add key="SheetID" value='tableIdValue'/>
</appSettings>
```

#### Пример:

```
<appSettings>
<add key="SheetID" value="1Zr4eMVwSUPpZz6Ts2UTiUZV1Lh2Ha0SRZqhHlnKPbF8" />
</appSettings>
``` 


# Пример корректного App.config

``` 
<connectionStrings>
<add name="serverName" providerName="Npgsql" connectionString="Server=localhost;Port=5432;User ID=admin;Password=12345;Database=database1;" />
</connectionStrings>

<appSettings>
<add key="SheetID" value="1Zr4eMVwSUPpZz6Ts2UTiUZV1Lh2Ha0SRZqhHlnKPbF8" />
<add key="serverSize" value="3" />
add key="credentials" value='{"installed":{"client_id":"766086565348-p2n1fk5q041tgfvqak5tgschsvkn7a2c.apps.googleusercontent.com","project_id":"quickstart-1585666415280","auth_uri":"https://accounts.google.com/o/oauth2/auth","token_uri":"https://oauth2.googleapis.com/token","auth_provider_x509_cert_url":"https://www.googleapis.com/oauth2/v1/certs","client_secret":"M5wm08jwi3BX3ZFWjb29ZdAM","redirect_uris":["urn:ietf:wg:oauth:2.0:oob","http://localhost"]}}'/>
</appSettings>

``` 
