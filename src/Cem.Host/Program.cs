using Cem.Host.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.AddCemHost();

var app = builder.Build();
app.UseCemHost();

app.Run();
