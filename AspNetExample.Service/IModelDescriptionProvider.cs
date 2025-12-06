namespace AspNetExample.Service;

public interface IModelDescriptionProvider
{
    string GetDescription(Guid modelId);
}