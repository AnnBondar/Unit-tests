using AspNetExample.Domain.Repositories;
using AspNetExample.Service.Models;
using AspNetExample.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using DomainModel = AspNetExample.Domain.Models.MyModel;

namespace AspNetExample.Service.Test;

[TestFixture]
public class ServiceTest
{
    private Mock<IRepository> _repositoryMock;
    private Mock<IModelDescriptionProvider> _modelDescriptionProviderMock;
    private IService _service;

    private const string CreateModelMethodName = nameof(IService.CreateModel) + ". ";
    private const string GetModelMethodName = nameof(IService.GetModel) + ". ";
    private const string UpdateModelMethodName = nameof(IService.UpdateModel) + ". ";
    private const string DeleteModelMethodName = nameof(IService.DeleteModel) + ". ";

    private readonly Guid _id = Guid.Parse("75D50016-466F-4030-9EAE-4D8D690C9957");
    private const string Data = "Test Data";
    
    [SetUp]
    public void Setup()
    {
        ServiceCollection services = new();
        services.RegisterService();

        _repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
        services.AddSingleton<IRepository>(_repositoryMock.Object);
        _modelDescriptionProviderMock = new Mock<IModelDescriptionProvider>(MockBehavior.Strict);
        services.AddSingleton<IModelDescriptionProvider>(_modelDescriptionProviderMock.Object);

        var serviceProvider = services.BuildServiceProvider();

        _service = serviceProvider.GetRequiredService<IService>();
    }

    [TestCase( Data, Description = "Create model with data", TestName = CreateModelMethodName + "With data")]
    [TestCase(null, Description = "Create model without data", TestName = CreateModelMethodName + "Without data")]
    public void CreateModel(string? data)
    {
        // Arrange
        DomainModel? domainModel = data is null ? null : new DomainModel(data, _id);
        _repositoryMock.Setup(x => x.CreateModel(data)).Returns(domainModel);

        // Act
        MyModel? actual = data is null ? null : _service.CreateModel(data);

        // Assert
        MyModel? expected = data is null ? null : new MyModel(data, _id);
        Assert.That(actual, Is.EqualTo(expected));
        if (data != null)
        {
            _repositoryMock.Verify(x => x.CreateModel(data), Times.Once);
        }
    }

    [TestCase(true, Description = "GetModel returns existing model", TestName = GetModelMethodName + "Found")]
    [TestCase(false, Description = "GetModel returns null when not found", TestName = GetModelMethodName + "Not found")]
    public void GetModel(bool exists)
    {
        // Arrange
        var domainModel = exists ? new DomainModel( Data, _id) : null;
        var description = exists ? "Test description" : null;

        _repositoryMock.Setup(x => x.GetModel(_id)).Returns(domainModel);
        if (exists)
        {
            _modelDescriptionProviderMock.Setup(x => x.GetDescription(_id)).Returns(description!);
        }

        // Act
        var actual = _service.GetModel(_id);

        // Assert
        MyModel? expected = exists ? new MyModel( Data, _id) { Description = description } : null;

        Assert.That(actual, Is.EqualTo(expected));
        _repositoryMock.Verify(x => x.GetModel(_id), Times.Once);
        if (exists)
        {
            _modelDescriptionProviderMock.Verify(x => x.GetDescription(_id), Times.Once);
        }
    }

    [TestCase( Data, Description = "UpdateModel returns updated model", TestName = UpdateModelMethodName + " Model updated")]
    public void UpdateModel(string updatedData)
    {
        // Arrange
        var inputModel = new MyModel(updatedData, _id);
        var domainModel = new DomainModel(updatedData, _id);
        _repositoryMock.Setup(x => x.UpdateModel(It.IsAny<DomainModel>())).Returns(domainModel);

        // Act
        var actual = _service.UpdateModel(inputModel);

        // Assert
        var expected = new MyModel(updatedData, _id);
        Assert.That(actual, Is.EqualTo(expected));
        _repositoryMock.Verify(x =>
            x.UpdateModel(It.Is<DomainModel>(m =>
                m.Id == _id && m.Data == updatedData)), Times.Once);
    }

    [TestCase(true, Description = "DeleteModel returns true when model is deleted", TestName = DeleteModelMethodName + "Deleted")]
    [TestCase(false, Description = "DeleteModel returns false when model not found", TestName = DeleteModelMethodName + "NotFound")]
    public void DeleteModel(bool exists)
    {
        // Arrange
        var expected = exists;
        _repositoryMock.Setup(x => x.DeleteModel(_id)).Returns(expected);

        // Act
        var actual = _service.DeleteModel(_id);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
        _repositoryMock.Verify(x => x.DeleteModel(_id), Times.Once);
    }
}