using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate void TileClickedEvent(object sender, object args);

public class InputController : MonoBehaviour
{
    public TileClickedEvent tileClicked = delegate { };
    public TileClickedEvent returnClicked = delegate { };

    public static InputController instance;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            returnClicked(null, null);
        }
    }
}
