using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;

namespace ConfigSnapperTests;

public class ConfigSnapperTests
{
    [Fact]
    public async Task GitIsInstalled()
    {
        var container = await CreateContainer();

        var result = await container.ExecAsync(["git, --version"]);

        Assert.StartsWith("git version", result.Stdout);
    }

    private async Task<IFutureDockerImage> CreateImage()
    {
        Console.WriteLine("Creating image...");
        var image = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), "ConfigSnapper")
            .WithDockerfile("Dockerfile")
            .WithDeleteIfExists(true)
            .Build();

        await image.CreateAsync();
        return image;
    }

    private async Task<IContainer> CreateContainer()
    {
        var image = await CreateImage();
        Console.WriteLine("Creating container...");
        return new ContainerBuilder()
            .WithImage(image)
            .WithName("ConfigSnapperTest")
            .WithAutoRemove(true)
            .Build();
    }
}