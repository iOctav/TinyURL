using Microsoft.OpenApi.Models;
using TinyURL.Repositories;
using TinyURL.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo { Title = "gRPC transcoding", Version = "v1" });
});
builder.Services.AddSingleton<ITinyUrlRepository, TinyUrlRepository>();
builder.Services.AddTransient<ITinyUrlService, TinyUrlService>();

var app = builder.Build();

app.UseSwagger();
if (app.Environment.IsDevelopment())
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
// Configure the HTTP request pipeline.
app.MapGrpcService<TinyUrlGrpcService>();


app.Run();