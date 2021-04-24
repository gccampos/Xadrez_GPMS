﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnMovement : Movement
{
    public override List<Tile> GetValidMoves()
    {
        Vector2Int direction = GetDirection();
        List<Tile> moveable=GetPawnAttack(direction);
        List<Tile>moves;
        if(!Chessboard.instance.selectedPiece.wasMoved){
            moves=UntilBlockedPath(direction,false,2);
            SetNormalMove(moves);
            if(moves.Count==2){
                moves[1].moveType=MoveType.PawnDoubleMove;
            }          
        }
        else{
            moves= moves=UntilBlockedPath(direction,false,1);
            SetNormalMove(moves);
        }
        moveable.AddRange(moves);
        CheckPromotion(moves);

        return moveable;
    }

    Vector2Int GetDirection()
    {
        if (StateMachineController.instance.currentlyPlaying.transform.name == "GreenPieces")
            return new Vector2Int(0, -1);
        return new Vector2Int(0, 1);
    }

    List<Tile> GetPawnAttack(Vector2Int direction){
        List<Tile> pawnAttack=new  List<Tile>();
        Tile temp;
        Piece piece= Chessboard.instance.selectedPiece;
        Vector2Int leftPos=new Vector2Int(piece.tile.pos.x-1,piece.tile.pos.y+ direction.y);
        Vector2Int rightPos=new Vector2Int(piece.tile.pos.x+1,piece.tile.pos.y+ direction.y);
        GetPawnAttack(GetTile(leftPos),pawnAttack);
        GetPawnAttack(GetTile(rightPos),pawnAttack);
        return pawnAttack;
    }
    void GetPawnAttack(Tile tile,List<Tile> pawnAttack ){
        if(tile==null){
            return;
        }
        if(isEnemy(tile)){
            tile.moveType=MoveType.Normal;
            pawnAttack.Add(tile);
        }else if(tile.moveType==MoveType.EnPassant){
            pawnAttack.Add(tile);
        }
    }
    void CheckPromotion(List<Tile> tiles){
         foreach(Tile t in tiles){
             if(t.pos.y==0 || t.pos.y==7){
                 t.moveType=MoveType.Promotion;
             }
         }
    }
}
