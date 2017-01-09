using System;

namespace Wink
{
    public interface ILocal
    {
        GameObject GetGameObjectByGUID(Guid guid);
    }
}
