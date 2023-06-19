using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewBehaviour : Navigator
{
    public delegate void OnMouseClickHandler( );
    public event OnMouseClickHandler OnMouseClick;

    private void OnMouseDown() {

        UIController.SelectedEntity = this;
        OnMouseClick?.Invoke( );
    }
}
