using Matiasg19.ConfigSnapper;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddConfigSnapper(builder.Configuration);

var host = builder.Build();

host.Services.UseConfigSnapper(builder.Configuration);

host.Start();
