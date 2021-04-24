using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
  public static AiController instance;
  public Ply currentState;
  void Awake(){
      instance=this;
  }
  [ContextMenu("Calculate Plays")]
  public async void CalculatePlays(){
      currentState=CreateSnapShot();
      currentState.name="start";
      EvaluateBoard(currentState);
      Ply currentPly=currentState;
      currentPly.originPly=null;
      currentPly.futurePlies=new List<Ply>();
      Debug.Log("Começo");
  }
 Ply CreateSnapShot(){
      Ply ply= new Ply();
      ply.golds= new List<PieceEvaluation>();
      ply.greens= new List<PieceEvaluation>();
      foreach(Piece p in Chessboard.instance.GoldenPieces){
          if(p.gameObject.activeSelf){
            ply.golds.Add(CreateEvaluationPiece(p,ply));  
          }
      }
      foreach(Piece p in Chessboard.instance.GreenPieces){
          if(p.gameObject.activeSelf){
            ply.greens.Add(CreateEvaluationPiece(p,ply));  
          }
      }
       return ply;
  }
  PieceEvaluation CreateEvaluationPiece(Piece piece, Ply ply){
      PieceEvaluation eva= new PieceEvaluation();
      eva.piece=piece;
      return eva;
  } 
  
   void EvaluateBoard(Ply ply){
      foreach(PieceEvaluation piece in ply.golds ){
          EvaluatePiece(piece,ply,1);
      }
       foreach(PieceEvaluation piece in ply.greens ){
          EvaluatePiece(piece,ply,-1);
      }
      Debug.Log("Chessboard score: "+ply.score);
  }
  void EvaluatePiece(PieceEvaluation eva, Ply ply,int scoreDirection){
      Chessboard.instance.selectedPiece=eva.piece;
      eva.availableMoves=eva.piece.movement.GetValidMoves();
      eva.score=eva.piece.movement.value;
      ply.score+=eva.score*scoreDirection;
  }
  void ResetBoard(Ply ply){
     foreach(AffectedPiece p in ply.changes){
         p.piece.tile.content=null;
         p.piece.tile=p.from;
         p.from.content=p.piece;
         p.piece.transform.position= new Vector3(p.from.pos.x, p.from.pos.y,0);
         p.piece.gameObject.SetActive(true);
     }
  }
  [ContextMenu("Reset test")]
   void ResetBoard(){
      currentState.changes=PieceMovementState.changes;
      ResetBoard(currentState);
   }
}
