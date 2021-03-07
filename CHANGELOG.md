## 2.25.0 NOVA 2 Delivery 8 (March 01, 2021)

* LT-3085: Chest crashed - Post Incident Investigation

## 2.24.0 NOVA 2 Delivery 7 (February 01, 2021)

* LT-3046: Update contracts accordingly to new half working day feature 

## 2.23.1 + client-3.13.0 + client-3.14.0 (November 26, 2020)

* LT-2815: Update HttpClientGenerator lib to get HTTP 400 details

## 2.23.0 (November 23, 2020) NOVA 2. Delivery 4.

* LT-2697: Strange new errors in Chest since last delivery

## 2.22.0 + Client 3.10.2 (October 26, 2020) NOVA 2. Delivery 3.

* LT-2566: Implement api for GUI in chest
* LT-2671: Order AuditTrail records by Timestamp descending

## 2.21.0 + Client 3.9.2 (September 21, 2020) NOVA 2. Delivery 2.

* LT-2506: Add contract for locales api
* LT-2507: Implement CRUD api for locales in Chest
* LT-2522: Localize currencies and categories

### Settings changes

* Add new section to appsettings.json

```json
"CqrsSettings": {
    "RetryDelay": "00:00:02",
    "EnvironmentName": "dev"
  }
```
* Add connection string to RabbitMq

Add CqrsSettings:ConnectionString to appsettings.json / secrets.json or add CQRS_CONNECTIONSTRING environment variable

### RabbitMQ changes.
New bindings to the queue dev.SettingsService.events.exchange

* to queue dev.Chest.queue.SettingsService.events.CurrencyChangedEvent.projections with routingKey CurrencyChangedEvent
* to queue dev.Chest.queue.SettingsService.events.ProductCategoryChangedEvent.projections with routing key ProductCategoryChangedEvent

## 2.20.0 (August 31, 2020) NOVA 2. Delivery 1.

* LT-2460: Implement localization api in Chest

## 2.15.2 (August 21, 2020)

* LT-2435: fix BulkReplace method

## 2.15.1 (August 11, 2020)

* LT-2370: Chest database error

## 2.15.0 + Client 3.6.0 (June 04, 2020)

* LT-2260: Migrate to 3.1 Core and update DL libraries

## 2.14.3 + Client 3.5.1 (May 11, 2020)

* LT-2063: Update new Lykke.Middlewares nuget
* LT-2199: Implement settings bulk update

## Client 3.1.4 (February 06, 2020)

LT-2038: Handle 404 HTTP response code in Chest client

## 2.14.2 (January 27, 2020)

* LT-1672: [Security] Chest accessible without login/password (part of LT-1671)

## 2.14.1 (December 11, 2019)

* LT-1754: Check that all services have .net core 2.2 in projects and docker
* LT-1823: [Logs Improvement] - Chest - Product Insertion

## 2.14.0 (July 08, 2019)

* LT-1541: Update licenses headers and add LICENSE  file

## 2.13.0 (April 10, 2019)

* LT-1240: Update Licenses.

## 2.12.0 (March 27, 2019)

* LT-1120: Fixed warnings for packages version and misusage, which also led to app crash in first web request
* LT-1210: Removed wrong error message when secrets provided from appsettings.json instead of user secrets

### Lykke.Snow.Common.Startup

Lykke.Snow.Common.Startup was updated and a new nuget version is published (Version 1.2.6)

Missing secret wrong error message is removed and a few more improvements made while adding environment variables and secrets

## 2.11.0 (March 8, 2019)

* LT-665: Moving (de)serialization to client to improve performance
* LT-391: Enhancing documentation for service requirements, including more detailed descriptions
* LT-397: Enhancing logging with correct app version and with Lykke middleware and standards
* LT-907: Removing private nuget sources from Nuget.config

### Chest Service

#### Configuration changes:

  - Added ability to specify Secrets variables via `appSettings.json`
  - Added ability to read global Nuget.config file injected in workspace folder and apply during docker image build

### Chest.Client

Chest Client was modified

 - Consolidated Bulk methods signature.
 - Moved (de)serialization to client to improve performance

## 2.10.0 (February 5, 2019)

* LT-834: Fixed BulkUpdate endpoint.

## 2.9.0 (January 14, 2019)

* LT-703 fixes for client serialization issues

## 2.8.1 (December 20, 2018)

* BUGS-188: Implementing second level cache for EF
* LT-620 fixed integration tests to run on SQL server database

## 2.8.0 (December 10, 2018)

* LT-466: Moved postgres password to test settings for chest fixture
* LT-518: Change Serilog sink from RollingFile to File
* LT-567: Add database connection string validation
* LT-601 Add bulk get and update endpoints for multiple asset editing
* LT-599: Multiple assets editing add check for assets always to be with upper case

### Configuration changes:

- Recommended to change timeout for SQL connection from default 30s value to at least 300s to avoid timeout problems during reports creation. 
Property in connection string: Connection Timeout=300;

- Update serilogs configuration. Exaple settings json configuration

```json 
{  
   "urls":"http://*:80;",
   "serilog":{  
      "minimumLevel":{  

      },
      "writeTo":[  
         {  
            "Name":"Async",
            "Args":{  
               "configure":[  
                  {  
                     "Name":"File",
                     "Args":{  
                        "rollingInterval":"Day",
                        "path":"logs/Chest/Chest-docker.log",
                        "fileSizeLimitBytes":null
                     }
                  }
               ]
            }
         }
      ]
   }
}
```
## X.X.X (December 30, 2019)

* LT-1672: Chest API key authentication.

### Configuration changes

- Added variable for Chest service API key. If variable is left unset or empty API call will be performed without authentication.

```none
  ChestClientSettings:ApiKey / CHEST_API_KEY 
```