namespace Application.Interfaces.Common
{
    public interface ICurrentUserContext
    {
        int GetUserId();
        string? GetIpAddress();
    }
}
