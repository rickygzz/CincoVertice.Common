namespace CincoVertice.Common.Utils.Config;

public interface IConfigFile
{
    void Delete(string keyName);

    string Get(string keyName);

    T Get<T>()
        where T : class;

    int GetKeyIndex(string key);

    string GetKeyName(int keyIndex);

    int GetValue(int keyIndex);

    ConfigTokenType GetValueType(int keyIndex);

    void Set(string keyName, string value);

    void Set<T>(T obj)
        where T : class, new();
}
