namespace Services.AssetManagement
{
    public interface IAssetProvider
    {
        T Load<T>(string name) where T : class;
    }
}