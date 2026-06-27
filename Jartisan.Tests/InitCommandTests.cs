using System.IO;
using Jartisan.Application.Ports;
using Jartisan.Application.UseCases.Init;
using Jartisan.Domain.Entities;
using Jartisan.CLI.Commands;

namespace Jartisan.Tests;

public class InitCommandTests
{
    [Fact]
    public void Execute_CallsJsonUseCase_WhenProjectAlreadyExists()
    {
        var factory = new FakeProjectFactory();
        var jsonManager = new FakeJsonManager();

        var detectUseCase = new InitDetectUseCase(new FakeProjectDetector(true));
        var createProjectUseCase = new InitProjectUseCase(factory);
        var jsonUseCase = new InitJsonUseCase(new FakeFolderScanner(new FolderMap(
            RootPath: "/tmp/test",
            Controllers: null,
            Models: null,
            Services: null,
            Repositories: null,
            Dtos: null,
            ScannedAt: DateTime.UtcNow)),
            jsonManager);

        var command = new InitCommand(detectUseCase, createProjectUseCase, jsonUseCase);

        using var output = new StringWriter();
        using var error = new StringWriter();
        Console.SetOut(output);
        Console.SetError(error);
        Console.SetIn(new StringReader(string.Empty));

        command.Execute();

        Assert.Equal(0, factory.CreateProjectCallCount);
        Assert.Equal(1, jsonManager.SaveTemplateCallCount);
    }

    [Fact]
    public void Execute_Cancels_WhenProjectDoesNotExistAndUserChoosesNo()
    {
        var detectUseCase = new InitDetectUseCase(new FakeProjectDetector(false));
        var createProjectUseCase = new InitProjectUseCase(new FakeProjectFactory());
        var jsonUseCase = new InitJsonUseCase(new FakeFolderScanner(new FolderMap(
            RootPath: "/tmp/test",
            Controllers: null,
            Models: null,
            Services: null,
            Repositories: null,
            Dtos: null,
            ScannedAt: DateTime.UtcNow)),
            new FakeJsonManager());

        var command = new InitCommand(detectUseCase, createProjectUseCase, jsonUseCase);

        using var output = new StringWriter();
        using var error = new StringWriter();
        Console.SetOut(output);
        Console.SetError(error);
        Console.SetIn(new StringReader("n\n"));

        command.Execute();

        string stderr = error.ToString();
        Assert.Contains("O projeto atual não é um projeto Maven", stderr);
        Assert.Contains("Operação cancelada pelo usuário", output.ToString());
    }

    [Fact]
    public void Execute_CreatesProjectAndSavesTemplate_WhenProjectDoesNotExistAndUserChoosesYes()
    {
        var detector = new FakeProjectDetector(false);
        var factory = new FakeProjectFactory();
        var jsonManager = new FakeJsonManager();
        var scanner = new FakeFolderScanner(new FolderMap(
            RootPath: "/tmp/test",
            Controllers: null,
            Models: null,
            Services: null,
            Repositories: null,
            Dtos: null,
            ScannedAt: DateTime.UtcNow));

        var detectUseCase = new InitDetectUseCase(detector);
        var createProjectUseCase = new InitProjectUseCase(factory);
        var jsonUseCase = new InitJsonUseCase(scanner, jsonManager);
        var command = new InitCommand(detectUseCase, createProjectUseCase, jsonUseCase);

        using var output = new StringWriter();
        using var error = new StringWriter();
        Console.SetOut(output);
        Console.SetError(error);
        Console.SetIn(new StringReader("s\n\n\n\n"));

        command.Execute();

        Assert.Equal(1, factory.CreateProjectCallCount);
        Assert.Equal(1, jsonManager.SaveTemplateCallCount);
        Assert.Contains("Criando projeto Maven", output.ToString());
    }

    private sealed class FakeProjectDetector : IProjectDetector
    {
        private readonly bool _projectExists;

        public FakeProjectDetector(bool projectExists)
        {
            _projectExists = projectExists;
        }

        public bool ProjectExists() => _projectExists;
    }

    private sealed class FakeProjectFactory : IProjectFactory
    {
        public int CreateProjectCallCount { get; private set; }

        public void CreateProject(JavaProjectConfig? config = null)
        {
            CreateProjectCallCount++;
        }
    }

    private sealed class FakeFolderScanner : IFolderScanner
    {
        private readonly FolderMap _folderMap;

        public FakeFolderScanner(FolderMap folderMap)
        {
            _folderMap = folderMap;
        }

        public FolderMap Scan() => _folderMap;
    }

    private sealed class FakeJsonManager : IJsonManager
    {
        public int SaveTemplateCallCount { get; private set; }

        public void SaveTemplate(FolderMap folderMap)
        {
            SaveTemplateCallCount++;
        }
    }
}
