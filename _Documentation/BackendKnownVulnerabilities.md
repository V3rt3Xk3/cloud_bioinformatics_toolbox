# Cloud_Bioinformatics_Toolbox Vulnerabilities

## BackEnd API Known vulnerabilities and Possible Counter Measures

### refreshToken Sniffing
* So the refresh token might gets sniffed. At this point I am unsure how secure HTTPS really is, so assume it can get sniffed.
* **Action**: So if it gets sniffed that will mean that both the **attacker** and the **User** has the same __refreshToken__. To gain access to the **User**'s data, the **attacker** would need to act fast, before a new __refreshToken__ would be issued. This in turn invalidates the **User**'s __refreshToken__.
  * In case, when the HTTPS channels are not broken or could not be broken, then to maintain access to the site, the **attacker** and the **User** would invalidate each others refreshTokens repeatedly. This could be catched. //FIXME: Waiting for implementation - mark if done.
  * Smart **attacker**: She/He could learn the **User**'s application usage patterns, so he would have a peaceful exploration time during the hack. Practically eaves dropping to all the traffic between the __cloud solution__ and the **User**, then she/he would only need to crack the encryption of the last __refreshToken__ exchange.
  * This however, might not be feasible from the **attacker** side. Also... If the __https__ traffic or the **User** secure key is leaked somehow, then there are much bigger problems in regard of the **User**. That's not something I think I can do about ATM as this key is not exchanged directly and out of the __scope of this project (ATM)__.
### The MongoDB controllers are vulnerable for attacks that exploit syncronicity / parallelicity
* This means, that in case of concurrent user registrations, the unique checking, which is a standalone DB call, might execute the same time, returning false responses, that in turn **lets two users register with the same username**. ATM this is considered an edge case.