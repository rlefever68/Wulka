namespace Wulka.BigD.Contract.Interfaces
{
    public interface IBigDServer
    {
        void CreateDatabase(string name);
        void Debug(string message);
        void DeleteAllDatabases();
        void DeleteDatabase(string name);
        void DeleteDatabases(string regExp);
        T GetDatabase<T>(string name) where T : IBigDDatabase, new();
        IBigDDatabase GetDatabase(string name);
        T GetDatabase<T>() where T : IBigDDatabase, new();
        System.Collections.Generic.IList<string> GetDatabaseNames();
        T GetExistingDatabase<T>() where T : IBigDDatabase, new();
        T GetExistingDatabase<T>(string name) where T : IBigDDatabase, new();
        IBigDDatabase GetNewDatabase(string name);
        T GetNewDatabase<T>(string name) where T : IBigDDatabase, new();
        bool HasDatabase(string name);
        IBigDRequest Request();
        string ServerName { get; }
        string DatabasePrefix { get; set; }
        bool RunningOnMono { get; }
        string Host { get; }
        int Port { get; }
        string UserName { get; }
        string Password { get; }
        string EncodedCredentials { get; }
    }
}
