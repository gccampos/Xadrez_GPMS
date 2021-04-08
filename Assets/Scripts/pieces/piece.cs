using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
      
public abstract class Piece : MonoBehaviour
{
    [HideInInspector]
    public Tile tile;
    public Movement movement;
    public bool wasMoved;

    void OnMouseDown(){
        Chessboard.instance.tileClicked(this, transform.parent.GetComponent<Player>());
    }
}
