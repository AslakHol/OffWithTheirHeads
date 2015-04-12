using UnityEngine;

public static class GameObjectUtils
{

    public static GameObject FindChild(this GameObject obj, string name)
    {
        GameObject returnObject = null;
        Transform transform = obj.transform;

        foreach (Transform trs in transform)
        {
            if (trs.gameObject.name == name)
            {
                returnObject = trs.gameObject;
                break;
            }

            returnObject = trs.gameObject.FindChild(name);

            if (returnObject)
                break;
        }

        return returnObject;
    }
}
