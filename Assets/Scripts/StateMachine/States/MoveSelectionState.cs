using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionState : State
{
    public override void Enter()
    {
        Debug.Log("MoveSelectionState");
        List<Tile> moves=new List<Tile>();
        Debug.Log(Chessboard.instance.selectedPiece.movement);
        moves = Chessboard.instance.selectedPiece.movement.GetValidMoves();
        Highlights.instance.SelectTiles(moves);
        Chessboard.instance.tileClicked+= OnHighlightClicked;

    }
     public override void Exit(){
        Highlights.instance.DeSelectTiles();
        Chessboard.instance.tileClicked -= OnHighlightClicked;
     }

     void OnHighlightClicked(object sender ,object args){
         HighlightClick highlight= sender  as  HighlightClick;
         if(highlight==null){
             return;
         }
         Vector3 v3Pos=highlight.transform.position;
         Vector2Int pos= new Vector2Int((int)v3Pos.x,(int)v3Pos.y-1);
         Tile tileClicked;
         if(Chessboard.instance.tiles.TryGetValue(pos,out tileClicked )){
             Debug.Log(tileClicked.pos);
             Chessboard.instance.selectedHighlight= highlight;
             machine.ChangeTo<PieceMovementState>();
         }
     }
     
}
