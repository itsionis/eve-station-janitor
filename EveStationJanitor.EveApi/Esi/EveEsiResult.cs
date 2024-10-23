using OneOf;
using OneOf.Types;

namespace EveStationJanitor.EveApi.Esi;

[GenerateOneOf]
public partial class EveEsiResult<T> : OneOfBase<T, Error<string>, NotModified>
{
    public bool IsError => IsT1;
    public T Result => AsT0;
}

public static class EveEsiResult
{
    public static EveEsiResult<T> FromResult<T>(OneOfBase<T, Error<string>, NotModified> result)
    {
        return result.Match<EveEsiResult<T>>(
            value => value,
            error => error,
            notModified => notModified);
    }
}
