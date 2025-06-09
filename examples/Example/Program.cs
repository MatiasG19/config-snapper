using Matiasg19.ConfigSnapper;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddConfigSnapper(builder.Configuration);

var h = builder.Build();

h.Services.UseConfigSnapper(builder.Configuration);

h.Start();

// Keep console running (press any key to shutdown)
Console.ReadLine();
