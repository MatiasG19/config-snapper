using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace ConfigSnapperTests;

public class ConfigSnapperTests
{
    [Fact]
    public async Task GitInstalled()
    {
        var container = new ContainerBuilder()
            .WithImage("mcr.microsoft.com/dotnet/runtime:5.0")
            .WithName("ConfigSnapper-Testcontainer")
            .WithEntrypoint("/bin/sh", "-c", "while :; do sleep 1; done")
            .WithExposedPort(80)
            .WithCleanUp(true)
            .WithCommand("/bin/sh", "-c", "apt-get update && apt-get install -y git && while :; do sleep 1; done")
            .Build();

        // Start the Testcontainer
        await container.StartAsync();
        Console.WriteLine("Testcontainer started.");

        static async Task<string> ExecuteCommandInContainer(IContainer container, string command)
        {
            var execResult = await container.ExecAsync(new[] { "/bin/sh", "-c", command });
            return execResult.Stdout;
        }

        var result = await ExecuteCommandInContainer(container, "git --version");

        Assert.StartsWith("git version", result);
    }
}