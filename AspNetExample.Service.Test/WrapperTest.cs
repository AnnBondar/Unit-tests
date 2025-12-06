using AspNetExample.Domain.Repositories;
using AspNetExample.Service.Models;
using AspNetExample.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using DomainModel = AspNetExample.Domain.Models.MyModel;

namespace AspNetExample.Service.Test;

[TestFixture]
public class WrapperTest
{
    private Mock<IRepository> _repositoryMock;
    private Mock<IModelDescriptionProvider> _descriptionProviderMock;
    private Wrapper _wrapper;
    
    private const string CreateModelMethodName = nameof(IService.CreateModel) + ". ";
    private const string GetModelMethodName = nameof(IService.GetModel) + ". ";
    private const string UpdateModelMethodName = nameof(IService.UpdateModel) + ". ";
    private const string DeleteModelMethodName = nameof(IService.DeleteModel) + ". ";
    
    private readonly Guid _id = Guid.Parse("75D50016-466F-4030-9EAE-4D8D690C9957");
    private const string Data = "Test Data";

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.RegisterService();

        _repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
        services.AddSingleton<IRepository>(_repositoryMock.Object);
        _descriptionProviderMock = new Mock<IModelDescriptionProvider>(MockBehavior.Strict);
        services.AddSingleton<IModelDescriptionProvider>(_descriptionProviderMock.Object);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<IService>();

        _wrapper = new Wrapper(service);
    }

    [TestCase(Data, Description = "Create model with data", TestName = CreateModelMethodName + "With data")]
    [TestCase(null, Description = "Create model without data", TestName = CreateModelMethodName + "Without data")]
    public void CreateModel(string? data)
    {
        // Arrange
        DomainModel? domainModel = data is null ? null : new DomainModel(data, _id);
        _repositoryMock.Setup(x => x.CreateModel(data)).Returns(domainModel);

        // Act
        MyModel? actual = data is null ? null : _wrapper.CreateModel(data);

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
        var domainModel = exists ? new DomainModel(Data, _id) : null;
        var description = exists ? "Test description" : null;

        _repositoryMock.Setup(x => x.GetModel(_id)).Returns(domainModel);
        if (exists)
        {
            _descriptionProviderMock.Setup(x => x.GetDescription(_id)).Returns(description!);
        }

        // Act
        var actual = _wrapper.GetModel(_id);

        // Assert
        MyModel? expected = exists ? new MyModel(Data, _id) { Description = description } : null;
        Assert.That(actual, Is.EqualTo(expected));
        _repositoryMock.Verify(x => x.GetModel(_id), Times.Once);
        if (exists)
        {
            _descriptionProviderMock.Verify(x => x.GetDescription(_id), Times.Once);
        }
    }

    [TestCase("Updated Data", Description = "UpdateModel returns updated model", TestName = UpdateModelMethodName + "Model updated")]
    public void UpdateModel(string updatedData)
    {
        // Arrange
        var inputModel = new MyModel(updatedData, _id);
        var domainModel = new DomainModel(updatedData, _id);
        _repositoryMock.Setup(x => x.UpdateModel(It.IsAny<DomainModel>())).Returns(domainModel);

        // Act
        var actual = _wrapper.UpdateModel(inputModel);

        // Assert
        var expected = new MyModel(updatedData, _id);
        Assert.That(actual, Is.EqualTo(expected));
        _repositoryMock.Verify(x =>
                x.UpdateModel(It.Is<DomainModel>(m => m.Id == _id && m.Data == updatedData)),
            Times.Once);
    }

    [TestCase(true, Description = "DeleteModel returns true when model is deleted", TestName = DeleteModelMethodName + "Deleted")]
    [TestCase(false, Description = "DeleteModel returns false when model not found", TestName = DeleteModelMethodName + "NotFound")]
    public void DeleteModel(bool exists)
    {
        // Arrange
        var expected = exists;
        _repositoryMock.Setup(x => x.DeleteModel(_id)).Returns(expected);

        // Act
        var actual = _wrapper.DeleteModel(_id);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
        _repositoryMock.Verify(x => x.DeleteModel(_id), Times.Once);
    }
}

internal class Wrapper
{
    private readonly IService _service;

    public Wrapper(IService service)
    {
        _service = service;
    }

    public MyModel CreateModel(string data) => _service.CreateModel(data);
    public MyModel GetModel(Guid id) => _service.GetModel(id);
    public MyModel UpdateModel(MyModel model) => _service.UpdateModel(model);
    public bool DeleteModel(Guid id) => _service.DeleteModel(id);
}
