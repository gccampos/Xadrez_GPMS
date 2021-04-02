using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightClick : MonoBehaviour
{
     void OnMouseDown() {
        Chessboard.instance.tileClicked(this,null);
    }
}
