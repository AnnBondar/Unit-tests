using AspNetExample.Domain.Models;
using AspNetExample.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using AspNetExample.Service;

namespace AspNetExample.Domain.Test;

[TestFixture]
public class RepositoryTest
{
    private IRepository _repository;
    private Mock<IGuidProvider> _guidProviderMock;
    private Mock<IStorage> _storageMock;

    private const string CreateModelMethodName = nameof(IRepository.CreateModel) + ". ";
    private const string GetModelMethodName = nameof(IRepository.GetModel) + ". ";
    private const string UpdateModelMethodName = nameof(IRepository.UpdateModel) + ". ";
    private const string DeleteModelMethodName = nameof(IRepository.DeleteModel) + ". ";

    private readonly Guid _id = Guid.Parse("75D50016-466F-4030-9EAE-4D8D690C9957");
    private const string Data = "Test Data";
    
    [SetUp]
    public void Setup()
    {
        ServiceCollection services = new();
        services.RegisterDomain();

        _guidProviderMock = new(MockBehavior.Strict);
        services.AddSingleton(_guidProviderMock.Object);

        _storageMock = new(MockBehavior.Strict);
        services.AddSingleton(_storageMock.Object);

        var serviceProvider = services.BuildServiceProvider();
        _repository = serviceProvider.GetRequiredService<IRepository>();
    }

    private void SetupStorage() => _storageMock.Setup(x => x.Models).Returns(new Dictionary<Guid, MyModel>());

    private void SetupGuidProvider() => _guidProviderMock.Setup(x => x.GetGuid()).Returns(_id);

    [TestCase(Data, Description = "Create model with data", TestName = CreateModelMethodName + "With data")]
    [TestCase(null, Description = "Create model without data", TestName = CreateModelMethodName + "Without data")]
    public void CreateModel(string data)
    {
        // Arrange
        SetupStorage();
        SetupGuidProvider();

        // Act
        var actual = _repository.CreateModel(data);

        // Assert
        MyModel expected = data is null ? null : new(data, _id);
        Assert.That(actual, Is.EqualTo(expected));
        _guidProviderMock.Verify(x => x.GetGuid(), data is null ? Times.Never : Times.Once);
        _storageMock.Verify(x => x.Models, data is null ? Times.Never : Times.Once);
    }

    [TestCase(true, Description = "GetModel returns existing model", TestName = GetModelMethodName + "Found")]
    [TestCase(false, Description = "GetModel returns null when not found", TestName = GetModelMethodName + "Not found")]
    public void GetModel(bool exists)
    {
        // Arrange
        var model = new MyModel(Data, _id);
        var dict = exists
            ? new Dictionary<Guid, MyModel> { { _id, model } }
            : new Dictionary<Guid, MyModel>();
        _storageMock.Setup(x => x.Models).Returns(dict);

        // Act
        var actual = _repository.GetModel(_id);

        // Assert
        var expected = exists ? model : null;
        Assert.That(actual, Is.EqualTo(expected));
        _storageMock.Verify(x => x.Models, Times.Once);
    }

    [TestCase(Description = "UpdateModel return updated model", TestName = UpdateModelMethodName + " Model updated")]
    public void UpdateModel()
    {
        // Arrange
        var dict = new Dictionary<Guid, MyModel>();
        _storageMock.Setup(x => x.Models).Returns(dict);

        var expected = new MyModel(Data, _id);

        // Act
        var actual = _repository.UpdateModel(expected);

        // Assert
        Assert.That(dict[_id], Is.EqualTo(expected));
        Assert.That(actual, Is.EqualTo(expected));
        _storageMock.Verify(x => x.Models, Times.Once);
    }

    [TestCase(true, Description = "DeleteModel removes existing model", TestName = DeleteModelMethodName + "Deleted")]
    [TestCase(false, Description = "DeleteModel returns false when model not found", TestName = DeleteModelMethodName + "Not found")]
    public void DeleteModel(bool exists)
    {
        // Arrange
        var model = new MyModel(Data, _id);
        var dict = exists ? new Dictionary<Guid, MyModel> { { _id, model } } : new Dictionary<Guid, MyModel>();
        _storageMock.Setup(x => x.Models).Returns(dict);

        // Act
        var actual = _repository.DeleteModel(_id);

        // Assert
        var expected = exists;
        Assert.That(actual, Is.EqualTo(expected));
        Assert.That(dict.ContainsKey(_id), Is.False);
        _storageMock.Verify(x => x.Models, Times.Once);
    }
}