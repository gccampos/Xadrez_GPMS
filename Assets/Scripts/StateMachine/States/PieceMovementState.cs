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

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            float timing = Vector3.Distance(piece.transform.position,pos)*0.5f;
            LeanTween.move(piece.gameObject,pos, 1.5f).
                setOnComplete(()=> {
                    tcs.SetResult(true);
                });
            await tcs.Task;
            machine.ChangeTo<TurnEndState>();
    }
}
