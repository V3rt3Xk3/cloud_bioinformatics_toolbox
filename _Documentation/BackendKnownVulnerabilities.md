# Cloud_Bioinformatics_Toolbox Backend Vulnerabilities

## BackEnd API Known vulnerabilities and Possible Counter Measures

### refreshToken Sniffing
* So the refresh token might gets sniffed. At this point I am unsure how secure HTTPS really is, so assume it can get sniffed. //VULN: Read on man-in-the-middle attacks - mark if done.
* **Action**: So if it gets sniffed that will mean that both the **attacker** and the **User** has the same _refreshToken_. To gain access to the **User**'s data, the **attacker** would need to act fast, before a new _refreshToken_ would be issued. This in turn invalidates the **User**'s _refreshToken_.
  * In case, when the HTTPS channels are not broken or could not be broken, then to maintain access to the site, the **attacker** and the **User** would invalidate each others refreshTokens repeatedly. This could be catched. //VULN: Waiting for implementation - mark if done.
  * Smart **attacker**: She/He could learn the **User**'s application usage patterns, so he would have a peaceful exploration time during the hack. Practically eaves dropping to all the traffic between the _cloud solution_ and the **User**, then she/he would only need to crack the encryption of the last _refreshToken_ exchange.
    * This however, might not be feasible from the **attacker** side. Also... If the _https_ traffic or the **User** secure key is leaked somehow, then there are much bigger problems in regard of the **User**. That's not something I think I can do about ATM as this key is not exchanged directly and out of the _scope of this project (ATM)_.

### Stale JWT accessToken retains access rights
* As of this point, there is no validation about user rights. //VULN: Waiting for implementation - mark if done
  * Even though, we only pass the userID in the token. The user could retain access to the site even after revoking her/his access.
  * **Counter measures** - This point I think i would need to implement an acess rights scheme in mongoDB, having extra information about what the **User** can and can't do. We make a DB call everytime anyway. So an _accessType_ field could resolve this issue.

### The MongoDB controllers are vulnerable for attacks that exploit syncronicity / parallelicity
* This means, that in case of concurrent user registrations, the unique checking, which is a standalone DB call, might execute the same time, returning false responses, that in turn **lets two users register with the same username**. ATM this is considered an edge case.