using Jartisan.Application.Ports;
using Jartisan.Application.UseCases.Init;
using Jartisan.Domain.Entities;
using Jartisan.Infrastructure.Services;

namespace Jartisan.Tests;

public class InitUseCasesTests
{
    [Fact]
    public void InitDetectUseCase_ReturnsProjectExistsFromDetector()
    {
        var detector = new FakeProjectDetector(true);
        var useCase = new InitDetectUseCase(detector);

        Assert.True(useCase.Execute());
    }

    [Fact]
    public void InitProjectUseCase_InvokesProjectFactory()
    {
        var factory = new FakeProjectFactory();
        var useCase = new InitProjectUseCase(factory);

        useCase.Execute(new JavaProjectConfig { GroupId = "com.example", ArtifactId = "app", Version = "1.0" });

        Assert.Equal(1, factory.CreateProjectCallCount);
    }

    [Fact]
    public void InitJsonUseCase_ScansFolderMapAndSavesTemplate()
    {
        var folderMap = new FolderMap(
            RootPath: "/tmp/test-root",
            Controllers: null,
            Models: null,
            Services: null,
            Repositories: null,
            Dtos: null,
            ScannedAt: DateTime.UtcNow);

        var scanner = new FakeFolderScanner(folderMap);
        var manager = new FakeJsonManager();
        var useCase = new InitJsonUseCase(scanner, manager);

        FolderMap result = useCase.Execute();

        Assert.Same(folderMap, result);
        Assert.Equal(1, manager.SaveTemplateCallCount);
        Assert.Same(folderMap, manager.LastSavedFolderMap);
    }

    private sealed class FakeProjectDetector : IProjectDetector
    {
        private readonly bool _result;

        public FakeProjectDetector(bool result)
        {
            _result = result;
        }

        public bool ProjectExists() => _result;
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
        public FolderMap? LastSavedFolderMap { get; private set; }

        public void SaveTemplate(FolderMap folderMap)
        {
            SaveTemplateCallCount++;
            LastSavedFolderMap = folderMap;
        }
    }
}
