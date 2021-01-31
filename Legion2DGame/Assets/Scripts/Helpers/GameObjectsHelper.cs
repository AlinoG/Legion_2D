using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectsHelper
{
    private static GameObject recursiveSearchObject = null;

    /// <summary>
    /// Recursively searches for specifc child in parent game object.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="specificChild"></param>
    /// <returns>GameObject or null</returns>
    public static GameObject RecursiveChildSearch(GameObject parent, string specificChild)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.name == specificChild)
            {
                recursiveSearchObject = child.gameObject;
                break;
            }
            else
            {
                RecursiveChildSearch(child.gameObject, specificChild);
            }
        }
        return recursiveSearchObject;
    }

    /// <summary>
    /// Finds child by given tag.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="tag"></param>
    /// <returns>GameObject or null</returns>
    public static GameObject FindChildByTag(GameObject parent, string tag)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }
        }

        return null;
    }
}
