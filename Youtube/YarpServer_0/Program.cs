var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", (int? a, int? b) => $"This is YarpServer_0 {(a ?? 0) + (b ?? 0)}");
// localhost:2000/?a=10&b=5 입력해서 YarpServer_0뒤에 값 확인해보기

app.UseHttpsRedirection();

app.Run();
