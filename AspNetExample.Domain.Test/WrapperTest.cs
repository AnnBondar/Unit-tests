using AspNetExample.Domain.Models;
using AspNetExample.Domain.Repositories;
using Moq;

namespace AspNetExample.Domain.Test;

[TestFixture]
public class WrapperTest
{
    private Mock<IRepository> _repositoryMock;
    private Wrapper _wrapper;

    private const string CreateModelMethodName = nameof(IRepository.CreateModel) + ". ";
    private const string GetModelMethodName = nameof(IRepository.GetModel) + ". ";
    private const string UpdateModelMethodName = nameof(IRepository.UpdateModel) + ". ";
    private const string DeleteModelMethodName = nameof(IRepository.DeleteModel) + ". ";
    
    private readonly Guid _id = Guid.Parse("75D50016-466F-4030-9EAE-4D8D690C9957");

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new(MockBehavior.Strict);
        _wrapper = new Wrapper(_repositoryMock.Object);
    }
    
    private void SetupRepository(string data) => _repositoryMock
        .Setup(x => x.CreateModel(It.Is<string>(d => d == data)))
        .Returns(data is null ? null :new MyModel(data, _id));
    
    [TestCase("data", Description = "Create model with data", TestName = CreateModelMethodName + "With data")]
    [TestCase(null, Description = "Create model without data", TestName = CreateModelMethodName + "Without data")]
    public void CreateModel(string data)
    {
        // Arrange
        SetupRepository(data);

        // Act
        var actual = _wrapper.CreateModel(data);

        // Assert
        MyModel expected = data is null ? null : new(data, _id);
        Assert.That(actual, Is.EqualTo(expected));
        _repositoryMock.Verify(x => x.CreateModel(data), Times.Once);
    }

    [TestCase(true, Description = "GetModel returns existing model", TestName = GetModelMethodName + "Found")]
    [TestCase(false, Description = "GetModel returns null when not found", TestName = GetModelMethodName + "Not found")]
    public void GetModel(bool exists)
    {
        // Arrange
        MyModel? expected  = exists ? new MyModel("data", _id) : null;
        _repositoryMock.Setup(x => x.GetModel(_id)).Returns(expected);

        // Act
        var actual  = _wrapper.GetModel(_id);

        // Assert
        Assert.That(actual , Is.EqualTo(expected));
        _repositoryMock.Verify(x => x.GetModel(_id), Times.Once);
    }
    
    [TestCase("new data", Description = "UpdateModel return updated model", TestName = UpdateModelMethodName + " Model updated")]
    public void UpdateModel(string newData)
    {
        // Arrange
        var expected = new MyModel(newData, _id);
        _repositoryMock.Setup(x => x.UpdateModel(expected)).Returns(expected);

        // Act
        var actual = _wrapper.UpdateModel(expected);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
        _repositoryMock.Verify(x => x.UpdateModel(expected), Times.Once);
    }

    [TestCase(true, Description = "DeleteModel returns true when deleted", TestName = DeleteModelMethodName + "Deleted")]
    [TestCase(false, Description = "DeleteModel returns false when not found", TestName = DeleteModelMethodName + "Not found")]
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
    private readonly IRepository _repository;

    public Wrapper(IRepository repository)
    {
        _repository = repository;
    }

    public MyModel CreateModel(string data) => _repository.CreateModel(data);
    public MyModel GetModel(Guid id) => _repository.GetModel(id);
    public MyModel UpdateModel(MyModel data) => _repository.UpdateModel(data);
    public bool DeleteModel(Guid id) => _repository.DeleteModel(id);

}