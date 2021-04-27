﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionState : State
{
    public override void Enter()
    {
        Debug.Log("MoveSelectionState");
        List<AvailableMove> moves=Chessboard.instance.selectedPiece.movement.GetValidMoves();  
        Highlights.instance.SelectTiles(moves);
        InputController.instance.tileClicked+= OnHighlightClicked;
        InputController.instance.returnClicked+= ReturnClicked;
    }
    public override void Exit(){
        Highlights.instance.DeSelectTiles();
        InputController.instance.tileClicked-= OnHighlightClicked;
        InputController.instance.returnClicked-= ReturnClicked;
    }

    void OnHighlightClicked(object sender ,object args){
         HighlightClick highlight= sender  as  HighlightClick;
         if(highlight==null){
             return;
         }
         //Vector3 v3Pos=highlight.transform.position;
         //Vector2Int pos= new Vector2Int((int)v3Pos.x,(int)v3Pos.y-1);
         //Tile tileClicked=highlight.tile;
         Chessboard.instance.selectedMove= highlight.move;
         machine.ChangeTo<PieceMovementState>();
         
    }

     void ReturnClicked(object sender, object args){
         machine.ChangeTo<PieceSelectionState>();
     }

}
