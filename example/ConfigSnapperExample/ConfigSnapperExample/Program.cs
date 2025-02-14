using Matiasg19.ConfigSnapper;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
    .Build();

var snapperConfig = new Matiasg19.ConfigSnapper.Configuration.ConfigSnapper();
configuration.GetRequiredSection("ConfigSnapper").Bind(snapperConfig);

var snapper = new ConfigSnapper(snapperConfig);

snapper.CreateSnapshot();
