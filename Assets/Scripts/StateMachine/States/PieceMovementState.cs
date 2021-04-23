﻿using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMovementState : State
{
    public override async void Enter(){

        MoveType moveType=Chessboard.instance.selectedHighlight.tile.moveType;
        ClearEnPassants();
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        switch (moveType)
        {
            case MoveType.Normal:
                NormalMove(tcs);
                break;
            case MoveType.Castling:
                Castling(tcs);
                break;
            case MoveType.PawnDoubleMove:
                PawnDoubleMove(tcs);
                break;  
            case MoveType.EnPassant:
                EnPassant(tcs);
                break;      
        }
        await tcs.Task;
        machine.ChangeTo<TurnEndState>();
    }

    void NormalMove(TaskCompletionSource<bool> tcs){
        Piece piece= Chessboard.instance.selectedPiece;
        Vector2 pos=Chessboard.instance.selectedHighlight.transform.position;
        pos.y-=1;
        //piece.transform.position=pos;
        piece.tile.content=null;
        piece.tile=Chessboard.instance.selectedHighlight.tile;
        if(piece.tile.content!=null){
            Piece deadPiece= piece.tile.content;
            Debug.LogFormat("Peça {0} foi morta",deadPiece.transform);
            deadPiece.gameObject.SetActive(false);
        }
        piece.tile.content=piece;
        piece.wasMoved = true;

        float timing = Vector3.Distance(piece.transform.position,pos)*0.5f;
        LeanTween.move(piece.gameObject,pos, 0.5f).
            setOnComplete(()=> {
                tcs.SetResult(true);
            }
        );
    }

    void Castling(TaskCompletionSource<bool> tcs){
        Piece king = Chessboard.instance.selectedPiece;
        king.tile.content = null;
        Piece rook = Chessboard.instance.selectedHighlight.tile.content;
        rook.tile.content = null;
        Vector2Int direction = rook.tile.pos - king.tile.pos;

        if(direction.x>0){
            king.tile = Chessboard.instance.tiles[new Vector2Int(king.tile.pos.x+2, king.tile.pos.y)];
            rook.tile = Chessboard.instance.tiles[new Vector2Int(king.tile.pos.x-1, king.tile.pos.y)];
        }else{
            king.tile = Chessboard.instance.tiles[new Vector2Int(king.tile.pos.x-2, king.tile.pos.y)];
            rook.tile = Chessboard.instance.tiles[new Vector2Int(king.tile.pos.x+1, king.tile.pos.y)];
        }
        king.tile.content = king;
        rook.tile.content = rook;
        king.wasMoved = true;
        rook.wasMoved = true;
        LeanTween.move(king.gameObject,new Vector3(king.tile.pos.x,king.tile.pos.y,0), 1.5f).
            setOnComplete(()=> {
                tcs.SetResult(true);
            }
        );
        LeanTween.move(rook.gameObject,new Vector3(rook.tile.pos.x,rook.tile.pos.y,0), 1.4f);
    }
    void ClearEnPassants(){
        ClearEnPassants(5);
        ClearEnPassants(2);
    }
    void ClearEnPassants(int height){
        Vector2Int positions=new Vector2Int(0,height);
        for(int i=0;i<7;i++){
            positions.x=positions.x+1;
            Chessboard.instance.tiles[positions].moveType=MoveType.Normal;
        }
    }
    void PawnDoubleMove(TaskCompletionSource<bool> tcs){
        Piece pawn= Chessboard.instance.selectedPiece;
        Vector2Int direction= pawn.tile.pos.y > Chessboard.instance.selectedHighlight.tile.pos.y? 
        new Vector2Int(0,-1):new Vector2Int(0,1);
        Chessboard.instance.tiles[pawn.tile.pos+direction].moveType=MoveType.EnPassant;
        NormalMove(tcs);
    }
    void EnPassant(TaskCompletionSource<bool> tcs){
        Piece pawn= Chessboard.instance.selectedPiece;
        Vector2Int direction= pawn.tile.pos.y > Chessboard.instance.selectedHighlight.tile.pos.y? 
        new Vector2Int(0,1):new Vector2Int(0,-1);
        Tile enemy=Chessboard.instance.tiles[Chessboard.instance.selectedHighlight.tile.pos+direction];
        enemy.content.gameObject.SetActive(false);
        enemy.content=null;
        NormalMove(tcs);
    }
}
