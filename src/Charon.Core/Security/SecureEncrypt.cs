using System.Reflection;
using Charon.Types;
using Serilog;

namespace Charon.Security;

public static partial class SecureEncrypt
{
    private static readonly SecurityLevel[] _levels =
    [
            new SecurityLevel5(),
            new SecurityLevel4(),
            new SecurityLevel3(),
            new SecurityLevel2()
    ];

    public static IPrivateKeyRetriever? PrivateKeyRetriever { get; set; }

    public static IPublicKeyRetriever? PublicKeyRetriever { get; set; }

    public static bool IsEncrypted(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        foreach (var level in _levels)
        {
            if (level.Match(value))
                return true;
        }

        return false;
    }

    public static bool IsSecureEncrypted(this string? value)
    {
        return !string.IsNullOrEmpty(value) &&
            _levels[0].Match(value);
    }

    public static string Encrypt(string value, string? stage)
    {
        var retriever = PublicKeyRetriever ??= FindKeyRetriever<IPublicKeyRetriever>();

        return Encrypt(value, stage, retriever);
    }

    public static string Encrypt(string value, string? stage, IPublicKeyRetriever retriever)
    {
        var key = retriever.GetKey(stage ?? retriever.DefaultStage);

        if (key == null)
            return value;

        return _levels[0].Encrypt(value, key);
    }

    public static string Decrypt(string value)
    {
        if (value.Length <= SecurityLevel.MinEncryptedValueLength ||
            !IsEncrypted(value))
            return value;

        var retriever = PrivateKeyRetriever ??= FindKeyRetriever<IPrivateKeyRetriever>();

        return Decrypt(value, retriever);
    }

    public static string Decrypt(string value, IPrivateKeyRetriever retriever)
    {
        if (value.Length <= SecurityLevel.MinEncryptedValueLength ||
            !IsEncrypted(value))
            return value;

        var key = retriever.GetKey();

        if (key == null)
            return value;

        var level = _levels.First(l => l.Match(value));
        var hash = retriever.GetHash();

        return level.Decrypt(value, key, hash);
    }

    private static T FindKeyRetriever<T>()
    {
        var types = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => t.IsPublic && !t.IsInterface && !t.IsAbstract && typeof(T).IsAssignableFrom(t))
            .OrderByDescending(s => s.GetCustomAttribute<PriorityAttribute>()?.Priority ?? int.MinValue)
            .ToArray();

        var type = types[0];

        Log.Information("Found '{TypeName}' as implementation for '{SearchTypeName}'", type.FullName, typeof(T).FullName);

        return (T)Activator.CreateInstance(type)!;
    }
}
