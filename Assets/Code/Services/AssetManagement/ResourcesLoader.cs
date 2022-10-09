using Services.AssetManagement;
using UnityEngine;

namespace Services.AssetManagement
{
    public class ResourcesLoader : IAssetProvider
    {
        public T Load<T>(string name) where T : class
        {
            return Resources.Load(name) as T;
        }
    }   
}