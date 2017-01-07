using System;

namespace Wink
{
    interface ILocal
    {
        GameObject GetGameObjectByGUID(Guid guid);
    }
}
