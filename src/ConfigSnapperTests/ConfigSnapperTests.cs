using DotNet.Testcontainers.Builders;

namespace ConfigSnapperTests;

public class ConfigSnapperTests
{
    [Fact]
    public async Task GitInstalled()
    {
        await CreateImage();
        var container = CreateContainer();

        var result = await container.ExecAsync(["git, --version"]);

        Assert.StartsWith("git version", result.Stdout);
    }

    private async Task CreateImage()
    {
        Console.WriteLine("Creating image...");
        var image = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
            .WithDockerfile("Dockerfile")
            .WithDeleteIfExists(true)
            .WithName("ConfigSnapperTest")
            .Build();

        await image.CreateAsync();

    }

    private DotNet.Testcontainers.Containers.IContainer CreateContainer()
    {
        Console.WriteLine("Creating container...");
        return new ContainerBuilder()
            .WithImage("ConfigSnapperTest")
            .Build();
    }
}