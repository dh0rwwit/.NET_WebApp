# .NET_WebApp
- ASP.NET core
- postgresql
- redis
- react
- IDE : visual studio 2022
- OS : Server on windows, Linux might be later...

0. Plan
- ASP.NET core with react UI
- util : YARP,cache, Blazor, MAUI

1. Architecture Planed

    Client(Seperate with Server IP)
   
    ↓
   
    [Load Balancer + WAF + SSL Termination]
   
    ↓
   
    ServerA / ServerB - 2 Server routing (1 read, 1 update) (Stateless, API, secure)
   
    ↓
   
    [NoSQL: Redis/Kafka/MongoDB (TLS, Authenticate)]...maybe Redis
   
    ↓
   
    ServerC (Consumer or Worker with Retry, Idempotent)
   
    ↓
   
    RDB (pgsql, SSL, Backup)

3. Yarp ->Reverse Proxy, Load balance
  - WAF, SSL Termination
  - Stateless
    
3. Nosql - RDB Consumer with Retry, IDempotent 

4. Requests from two clients are distributed between two servers. These two servers send the data to Redis, and the data in Redis is then used to update PostgreSQL.
– This part needs to be revised.
– Further research is needed on how to optimize data delivery.

5. Progress...

2025.07.21 _ Initialization(Client - Server Connect test)

2025.07.25 _ Local DB- Local server Connect

2025.07.31 _ service class using interface(factory)

2025.08.07 _ build postgresql for outter Access



