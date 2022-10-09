namespace Services.Saving
{
    public interface IDataSaver
    {
        T Load<T>(string filename) where T : class;
        void Save<T>(string filename, T data) where T : class;
        bool SaveExists(string path);
    }
}