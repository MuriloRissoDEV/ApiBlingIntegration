using ApiBling.Application.Interfaces;
using ApiBling.Application.Services;
using ApiBling.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<IBlingApiService, BlingApiService>()
    .AddPolicyHandler(HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests) // Intercepta o Erro 429 do Bling
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2 * retryAttempt))); // Se der erro, espera 2s, depois 4s, depois 6s e tenta de novo automaticamente

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISincronizacaoService, SincronizacaoService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
