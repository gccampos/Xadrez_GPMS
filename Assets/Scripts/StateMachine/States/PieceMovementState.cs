using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMovementState : State
{
    public static List<AffectedPiece> changes;
    public override async void Enter(){
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        MovePiece(tcs,false);
        await tcs.Task;
        machine.ChangeTo<TurnEndState>();
    }
   public static void MovePiece(TaskCompletionSource<bool> tcs, bool skipMovements){
      changes= new List<AffectedPiece>();
      MoveType moveType=Chessboard.instance.selectedHighlight.tile.moveType;
      ClearEnPassants(); 
        ClearEnPassants();
      ClearEnPassants(); 
      switch (moveType)
         {
            case MoveType.Normal:
                NormalMove(tcs, skipMovements);
                break;
            case MoveType.Castling:
                Castling(tcs, skipMovements);
                break;
            case MoveType.PawnDoubleMove:
                PawnDoubleMove(tcs, skipMovements);
                break;  
            case MoveType.EnPassant:
                EnPassant(tcs ,skipMovements);
                break;
            case MoveType.Promotion:
                Promotion(tcs ,skipMovements);
                break;          
         }

   }
    static void NormalMove(TaskCompletionSource<bool> tcs , bool skipMovements){
        Piece piece= Chessboard.instance.selectedPiece;
        AffectedPiece pieceMoving= new AffectedPiece();
        pieceMoving.piece=piece;
        pieceMoving.from=piece.tile;
        pieceMoving.to=Chessboard.instance.selectedHighlight.tile;
        pieceMoving.wasMoved=piece.wasMoved;
        changes.Add(pieceMoving);
        Vector2 pos=Chessboard.instance.selectedHighlight.transform.position;
        pos.y-=1;
        piece.tile.content=null;
        piece.tile=Chessboard.instance.selectedHighlight.tile;
        if(piece.tile.content!=null){
            Piece deadPiece= piece.tile.content;
            AffectedPiece pieceKilled= new AffectedPiece();
            pieceKilled.piece=deadPiece;
            pieceKilled.from=pieceKilled.to=piece.tile;
            changes.Add(pieceKilled);
            Debug.LogFormat("Peça {0} foi morta",deadPiece.transform);
            deadPiece.gameObject.SetActive(false);
        }
        piece.tile.content=piece;
        piece.wasMoved=true;       
        if(skipMovements){        
            //piece.transform.position=pos;
            tcs.SetResult(true);
        }else{
            piece.wasMoved = true;          
            float timing = Vector3.Distance(piece.transform.position,pos)*0.5f;
            LeanTween.move(piece.gameObject,pos, 0.5f).
                setOnComplete(()=> {
                    tcs.SetResult(true);
                }
            );
        }
    }

   static void Castling(TaskCompletionSource<bool> tcs , bool skipMovements){
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
   static void ClearEnPassants(){
        ClearEnPassants(5);
        ClearEnPassants(2);
    }
   static void ClearEnPassants(int height){
        Vector2Int positions=new Vector2Int(0,height);
        for(int i=0;i<7;i++){
            positions.x=positions.x+1;
            Chessboard.instance.tiles[positions].moveType=MoveType.Normal;
        }
    }
   static void PawnDoubleMove(TaskCompletionSource<bool> tcs, bool skipMovements){
        Piece pawn= Chessboard.instance.selectedPiece;
        Vector2Int direction= pawn.tile.pos.y > Chessboard.instance.selectedHighlight.tile.pos.y? 
        new Vector2Int(0,-1):new Vector2Int(0,1);
        Chessboard.instance.tiles[pawn.tile.pos+direction].moveType=MoveType.EnPassant;
        NormalMove(tcs, skipMovements);
    }
   static void EnPassant(TaskCompletionSource<bool> tcs, bool skipMovements){
        Piece pawn= Chessboard.instance.selectedPiece;
        Vector2Int direction= pawn.tile.pos.y > Chessboard.instance.selectedHighlight.tile.pos.y? 
        new Vector2Int(0,1):new Vector2Int(0,-1);
        Tile enemy=Chessboard.instance.tiles[Chessboard.instance.selectedHighlight.tile.pos+direction];
        enemy.content.gameObject.SetActive(false);
        enemy.content=null;
        NormalMove(tcs, skipMovements);
    }
   static async void Promotion(TaskCompletionSource<bool> tcs, bool skipMovements){
       TaskCompletionSource<bool> movementTCS= new TaskCompletionSource<bool>();
       NormalMove(movementTCS, skipMovements);
       await movementTCS.Task;
       Debug.Log("promoveu");
       StateMachineController.instance.taskHold= new TaskCompletionSource<object>();
       StateMachineController.instance.promotionPanel.SetActive(true);
       await StateMachineController.instance.taskHold.Task;
       string result =StateMachineController.instance.taskHold.Task.Result as string;
       if(result=="Knight"){
           Chessboard.instance.selectedPiece.movement= new KnightMovement();
           if(Chessboard.instance.selectedPiece.transform.parent.name=="GreenPieces"){
               Chessboard.instance.selectedPiece.GetComponent<SpriteRenderer>().sprite= Chessboard.instance.knightGreen;
           }else{
                Chessboard.instance.selectedPiece.GetComponent<SpriteRenderer>().sprite= Chessboard.instance.knightGolden;
           }
       }else{
            Chessboard.instance.selectedPiece.movement= new QueenMovement();
             if(Chessboard.instance.selectedPiece.transform.parent.name=="GreenPieces"){
               Chessboard.instance.selectedPiece.GetComponent<SpriteRenderer>().sprite= Chessboard.instance.queenGreen;
           }else{
                Chessboard.instance.selectedPiece.GetComponent<SpriteRenderer>().sprite= Chessboard.instance.queenGolden;
           }
       }
       StateMachineController.instance.promotionPanel.SetActive(false);
       tcs.SetResult(true);
    }
}
