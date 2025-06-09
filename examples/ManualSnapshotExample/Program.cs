using Matiasg19.ConfigSnapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddConfigSnapper(builder.Configuration);

var host = builder.Build();

var snapper = host.Services.GetRequiredService<Snapper>();

host.Start();

for (int i = 0; i < 10; i++)
{
    snapper.CreateSnapshot();
    Thread.Sleep(5000);
}

// Keep console running (press any key to shutdown)
Console.ReadLine();