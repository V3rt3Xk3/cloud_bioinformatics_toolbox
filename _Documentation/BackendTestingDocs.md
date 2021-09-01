# Cloud_Bioinformatics_Toolbox Testing

## BackEnd API

### MongoDB
MongoShell command line
$mongosh --authenticationDatabase "cloud_bioinformatics_test" -u "cloud_bioinformaitcs_mongo_dev" -p

* MongoDB server version: 5.0.1
* Mongosh: 1.0.1

### MongoDB integration tests TODO

* I need to test the UserServices, namely the refreshToken accessToken system.
  * Issuing new refreshTokens
  * Revoking refreshTokens
  * Rotating refreshTokens
  * Deleting refreshTokens

