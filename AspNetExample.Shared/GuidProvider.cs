namespace AspNetExample.Service;

internal class GuidProvider : IGuidProvider
{
    public Guid GetGuid() => Guid.CreateVersion7();
}