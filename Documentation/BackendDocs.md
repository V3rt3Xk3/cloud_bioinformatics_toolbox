# Cloud_Bioinformatics_Toolbox Docs

## Tutorial

URL: https://developer.okta.com/blog/2020/06/29/aspnet-core-mongodb

## .NET Versions
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
const DB_USER: string = "cloud_bioinformaitcs_mongo_dev";
const DB_PASS: string = encodeURIComponent("#33FalleN666#");
const mongo_URI: string = "mongodb://" + DB_USER + ":" + DB_PASS + "@localhost:27017/?authSource=cloud_bioinformatics";

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