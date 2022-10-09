using System.IO;
using UnityEngine;

namespace Services.Saving
{
    public class JsonSaver : IDataSaver
    {
        public T Load<T>(string filename) where T : class
        {
            var path = GetFilePath(filename);
            return SaveExists(filename) ? JsonUtility.FromJson<T>(File.ReadAllText(path)) : default;
        }

        public void Save<T>(string filename, T data) where T : class
        {
            var path = GetFilePath(filename);
            var text = JsonUtility.ToJson(data);
            File.WriteAllText(path, JsonUtility.ToJson(data));
        }

        public bool SaveExists(string filename) =>
            File.Exists(GetFilePath(filename));

        private string GetFilePath(string filename)
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            var path = Path.Combine(Application.dataPath, filename);
#elif UNITY_IOS || UNITY_ANDROID
        var path = Path.Combine(Application.persistentDataPath, filename);
#endif
            return path;
        }
    }
}