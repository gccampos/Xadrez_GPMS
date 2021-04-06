using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMovementState : State
{
    public override async void Enter(){
            Piece piece= Chessboard.instance.selectedPiece;
            Vector2 pos=Chessboard.instance.selectedHighlight.transform.position;
            pos.y-=1;
            piece.transform.position=pos;
            piece.tile.content=null;
            piece.tile=Chessboard.instance.selectedHighlight.tile;
            if(piece.tile.content!=null){
                Piece deadPiece= piece.tile.content;
                Debug.LogFormat("Peça {0} foi morta",deadPiece.transform);
                deadPiece.gameObject.SetActive(false);
            }
            piece.tile.content=piece;
            await Task.Delay(100);
            machine.ChangeTo<TurnEndState>();
    }
}
