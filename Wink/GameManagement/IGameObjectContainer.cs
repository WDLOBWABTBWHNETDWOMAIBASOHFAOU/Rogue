using System;
using System.Collections.Generic;

static class IGameObjectContainerExtensions
{
    public static GameObject Find(this IGameObjectContainer container, string id)
    {
        return container.Find((gobj) => gobj != null && gobj.Id == id);
    }
}

interface IGameObjectContainer
{
    List<GameObject> FindAll(Func<GameObject, bool> del);
    GameObject Find(Func<GameObject, bool> del);
}
