namespace dinopays.web.Starling
{
    public interface IStarlingClientFactory
    {
        IStarlingClient CreateClient(string accessToken);
    }
}