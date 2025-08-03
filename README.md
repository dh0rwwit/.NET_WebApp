# .NET_WebApp
처음부터 순서대로 해보기

ASP.NET core의 경우, Blazor부터 react UI적용 , YARP,Routing 등등 기능 확대해보기
계획은
1. 각 프레임워크 별 클라이언트를 서버에 연결
- react, Blazor Assembly

2. 서버 ASP.NET core MVC로 2개의 서버 생성 및 DB에 연결

3. Yarp를 활용하여 Reverse Proxy, Load balance 적용
  - WAF, SSL Termination
  - Stateless, 보안처리
    
4. Nosql - RDB Consumer with Retry, IDempotent 

5. 2개의 클라이언트에서 보내는 요청이 2개의 서버에 분할되어 전달되고, 이 2개의 서버는 Redis에 전달, Redis에전달된 데이터는 pgsql에 데이터 업데이트
- 이 부분은 계획 수정 필요
- 데이터 전달 최적화에대한 방법을 좀 더 알아봐야 함.
.......
</n>
</n>
2025.07.21 _ 기본 기능(클라이언트에서 요청시 서버에서 값 받아오는 것...)만 실행
</n>
</n>

2025.07.25 _ DB-서버 연결
</n>
</n>
2025.07.31 _ service구현체 interface 적용(factory)



