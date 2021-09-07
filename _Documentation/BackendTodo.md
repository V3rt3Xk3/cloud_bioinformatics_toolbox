# Cloud_Bioinformatics_Toolbox Backend Todo

## TODO

* Fix the De Morgen problem at removing old refreshTokens //BUG: This is a data trash problem. - Remove old refreshTokens
* Implement a JWT token white and black list. //VULN: Thre refreshToken sniffing attack could be mitigated this way, not solved, but mitigated.
* Implement Google Oauth2
  * Make user able to register and login with GMail!
* Use bcrypt for hashing, both passwords and both accessTokens. //VULN: This could be a vulnerability. Waiting for implementation. - Mark if Done
  * Read on the technology, maybe even on the history of it. //WOW: Read on Bcrypt and other hashing algorithms. - Mark if done!
* Read on Cross Site Request Forgery. //WOW: Read on it. - Mark if done!
* Document the Backend C# codebase, at least with the Intellisense xml
* Do i need to return document IDs to the frontend?