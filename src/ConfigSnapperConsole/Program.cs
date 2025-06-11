using Matiasg19.ConfigSnapper;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddConfigSnapper();

var host = builder.Build();

host.Start();

host.Services.UseConfigSnapper(builder.Configuration);