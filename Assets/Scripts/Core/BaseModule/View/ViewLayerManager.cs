using System.Collections.Generic;
using UnityEngine;


public class ViewLayerManager : MonoBehaviour
{
    public enum ViewLayer
    {
        Back,
        Main,
        Top,
        None
    }

    [SerializeField]
    public List<Transform> layers = new List<Transform>();

    public Transform GetLayerRoot(ViewLayer layer)
    {
        if (layer == ViewLayer.None)
            return null;
        return layers[(int)layer];
    }
}
