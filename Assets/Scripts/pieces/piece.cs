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
    public bool maxTeam;
    virtual protected void Start(){
        if(transform.parent.name=="GoldenPieces"){
            maxTeam=true;
        }
    }
    public virtual AffectedPiece CreateAffected(){
        return new AffectedPiece();
    }
    void OnMouseDown(){
        InputController.instance.tileClicked(this, transform.parent.GetComponent<Player>());
    }
}
