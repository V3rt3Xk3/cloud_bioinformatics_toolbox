# Cloud_Bioinformatics_Toolbox Docs

## Useful links for the app:

### NaturalDNA Sequences API

* https://localhost:5001/api/naturaldna

## Tutorials

URL: https://developer.okta.com/blog/2020/06/29/aspnet-core-mongodb

URL: https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-5.0&tabs=visual-studio

## Testing

URL: https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/testing?view=aspnetcore-5.0

## .NET

#### This how I initialized the project:

$dotnet new webapi -n Backend

#### Installing packages to Backend:

$dotnet add package MongoDB.Driver\
$dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect\
$dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson\

#### Installing packages to BackendTests:

$dotnet add package Moq\
$dotnet add package Microsoft.AspNetCore.Mvc.Testing\
$dotnet add package Xunit.Extensions.Ordering\

### .NET Versions

$dotnet --list-sdks

>5.0.302 [/usr/share/dotnet/sdk]

$dotnet --list-runtimes

>Microsoft.AspNetCore.App 5.0.8 [/usr/share/dotnet/shared/Microsoft.AspNetCore.App]\
>Microsoft.NETCore.App 5.0.8 [/usr/share/dotnet/shared/Microsoft.NETCore.App]

## MongoDB
MongoShell command line
$mongosh --authenticationDatabase "cloud_bioinformatics" -u "cloud_bioinformaitcs_mongo_dev" -p

* MongoDB server version: 5.0.1
* Mongosh: 1.0.1

### Mongo DevURI
const DB_USER: string = "cloud_bioinformaitcs_mongo_dev";\
const DB_PASS: string = encodeURIComponent("#33FalleN666#");\
const mongo_URI: string = "mongodb://" + DB_USER + ":" + DB_PASS + "@localhost:27017/?authSource=cloud_bioinformatics";\

### MongoDB Users
#### MongoDB Admin
* User= "mongo_admin"
* Pass= "theUsual"
* Database= -

#### MongoDB DBOwner
* User= "cloud_bioinformaitcs_mongo_owner"
* Pass= "#33FalleN666#"
* Database= "cloud_bioinformatics"

#### MongoDB readWrite
* User= "cloud_bioinformaitcs_mongo_dev"
* Pass= "#33FalleN666#"
* Database= "cloud_bioinformatics"

#### MongoDB readWrite _ TestingDB
* User= "cloud_bioinformaitcs_mongo_dev"
* Pass= "#33FalleN666#"
* Database= "cloud_bioinformatics_test"
* role: DBOwner