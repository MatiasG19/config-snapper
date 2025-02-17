using DotNet.Testcontainers.Containers;

namespace ConfigSnapperTests;

public class ConfigSnapperTests
{
    [Fact]
    public async Task Test1()
    {
        // Create a Testcontainer with a base image
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("mcr.microsoft.com/dotnet/runtime:5.0")
            .WithName("ConfigSnapper-Testcontainer")
            .WithEntrypoint("/bin/sh", "-c", "while :; do sleep 1; done")
            .WithExposedPort(80)
            .WithCleanUp(true)
            .WithCommand("/bin/sh", "-c", "apt-get update && apt-get install -y git && while :; do sleep 1; done");

        // Start the Testcontainer
        using (var testcontainer = testcontainersBuilder.Build())
        {
            await testcontainer.StartAsync();
            Console.WriteLine("Testcontainer started.");

            // Verify Git installation
            var result = await ExecuteCommandInContainer(testcontainer, "git --version");
            Console.WriteLine($"Git version: {result}");

            // Keep the container running
            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();

        }

        static async Task<string> ExecuteCommandInContainer(IContainer container, string command)
        {
            var execResult = await container.ExecAsync(new[] { "/bin/sh", "-c", command });
            return execResult.Stdout;
        }
    }
}