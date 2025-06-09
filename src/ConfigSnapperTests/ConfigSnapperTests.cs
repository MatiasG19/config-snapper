using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Images;

namespace ConfigSnapperTests;

public class ConfigSnapperTests
{
    [Fact]
    public async Task GitIsInstalled()
    {
        var container = CreateContainer();

        var result = await container.ExecAsync(["git, --version"]);

        Assert.StartsWith("git version", result.Stdout);
    }

    private IFutureDockerImage CreateImage()
    {
        Console.WriteLine("Creating image...");
        return new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
            .WithDockerfile("Dockerfile")
            .WithDeleteIfExists(true)
            .WithName("ConfigSnapperTest")
            .Build();
    }

    private DotNet.Testcontainers.Containers.IContainer CreateContainer()
    {
        var image = CreateImage();
        Console.WriteLine("Creating container...");
        return new ContainerBuilder()
            .WithImage(image)
            .WithName("ConfigSnapperTest")
            .WithAutoRemove(true)
            .Build();
    }
}