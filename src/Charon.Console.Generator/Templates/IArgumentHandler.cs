#nullable enable

using System;

namespace Charon.Console;

public interface IArgumentHandler<TGlobalOptions>
    where TGlobalOptions : IGlobalOptions
{
    void Process(string[] arguments, ref int index);

    void Execute(TGlobalOptions options);
}
