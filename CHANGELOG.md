## Unreleased

* LT-665: Moving (de)serialization to client to improve performance
* LT-391: Enhancing documentation for service requirements, including more detailed descriptions
* LT-397: Enhancing logging with correct app version and with Lykke middleware and standards
* LT-907: Removing private nuget sources from Nuget.config

### Configuration changes:

  - Added ability to specify Secrets variables via `appSettings.json`
  - Added ability to read global Nuget.config file injected in workspace folder and apply during docker image build

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
