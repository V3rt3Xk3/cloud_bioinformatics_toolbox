# Cloud_Bioinformatics_Toolbox Backend Docs

## BackEnd API

## Useful links for the app:

### NaturalDNA Sequences API

* https://localhost:5001/api/naturaldna \
* https://localhost:5001/api/users \

## Tutorials

URL: https://developer.okta.com/blog/2020/06/29/aspnet-core-mongodb \
URL: https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-5.0&tabs=visual-studio \

### User Authentication Tutorial

URL: https://jasonwatmore.com/post/2021/05/25/net-5-simple-api-for-authentication-registration-and-user-management \

#### JSON

Username: "vertex" \
Password: "#33FalleN666#" \
RePassword: "#33FalleN666#" \
\

Temp Token: "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjYxMWVkNzE2MjE2MWE0ZGFlMmRmY2RjOCIsIm5iZiI6MTYyOTQxMTc0NCwiZXhwIjoxNjMwMDE2NTQ0LCJpYXQiOjE2Mjk0MTE3NDR9.yAQWRPnYHgV0FLZ9qT7SH_X3KGWEcI-A06TPJGkdrKLLj9-_exjUi8vG48psbaI9H_ZoG7E_da_Tn2m1jXbmtA"


## Testing

URL: https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/testing?view=aspnetcore-5.0 \

### XUnit and Ordering

URL: https://github.com/tomaszeman/Xunit.Extensions.Ordering \

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

### Great MongoDB Tutorial, although for C

URL: https://www.codepoc.io/blog/c-sharp/6100/update-record-in-mongodb-using-c \
URL: https://chsakell.gitbook.io/mongodb-csharp-docs/crud-basics/ \

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