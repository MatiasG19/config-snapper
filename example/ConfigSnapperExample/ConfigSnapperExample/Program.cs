using Matiasg19.ConfigSnapper;

Console.WriteLine("Hello, World!");

var snapper = new ConfigSnapper(new Matiasg19.ConfigSnapper.Configuration.ConfigSnapper()
{
    SnapshotDirectory = "C:\\Users\\mgalu\\Downloads\\TestSnapshots",
    SnapConfigs = new Dictionary<string, string>
    {
        { "Test", "C:\\Users\\mgalu\\Downloads\\gaviscon.txt" }
    }

});

snapper.CreateSnapshot();
