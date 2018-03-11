namespace dinopays.web.Starling
{
    public class StarlingClientFactory : IStarlingClientFactory
    {

        public IStarlingClient CreateClient(string accessToken)
        {
            return new StarlingClient(accessToken);
        }
    }
}