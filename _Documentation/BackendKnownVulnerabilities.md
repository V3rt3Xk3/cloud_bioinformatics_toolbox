# Cloud_Bioinformatics_Toolbox Docs

## BackEnd API Known vulnerabilities

* The MongoDB controllers are vulnerable for attacks that exploit syncronicity / parallelicity
  * This means, that in case of concurrent user registrations, the unique checking, which is a standalone DB call, might execute the same time, returning false responses, that in turn **lets two users register with the same username**. ATM this is considered an edge case.