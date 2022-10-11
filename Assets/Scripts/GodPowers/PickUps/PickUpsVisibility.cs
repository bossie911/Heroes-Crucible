using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpsVisibility : MonoBehaviour
{
    [SerializeField]
    private bool _isVisible;

    [SerializeField]
    private MeshRenderer model;

    public bool IsVisible
    {
        get
        {
            return _isVisible;
        }
        set
        {
            _isVisible = value;
            Visibility();
        }
    }

    private void Visibility()
    {
        model.enabled = _isVisible;
    }
}
