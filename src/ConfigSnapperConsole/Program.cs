using Matiasg19.ConfigSnapper;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

string[] arguments = Environment.GetCommandLineArgs();
builder.Services.AddConfigSnapper(arguments.Length > 1 ? arguments[1] : null);

var host = builder.Build();

host.Start();

host.Services.UseConfigSnapper(builder.Configuration);