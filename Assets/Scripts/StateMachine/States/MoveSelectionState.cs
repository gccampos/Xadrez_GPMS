using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionState : State
{
    public override void Enter()
    {
        Debug.Log("MoveSelectionState");
        List<Tile> moves = Chessboard.instance.selectedPiece.movement.GetValidMoves();
        foreach(Tile t in moves){
            Debug.Log(t.pos);
        }
    }
}
