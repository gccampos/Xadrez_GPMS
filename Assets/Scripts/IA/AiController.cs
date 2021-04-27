using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
  public static AiController instance;
  public Ply currentState;
  public AvailableMove enPassantFlagSaved;
  int calculationCount;
  public int objectivePlyDepth=2;
  public HighlightClick AIhighlight;
  float lastInterval;
  void Awake(){
      instance=this;
  }
    async Task<Ply> CalculatePly(Ply parentPly,List<PieceEvaluation> team, int currentPlyDepth, int minMaxDirection){
        parentPly.futurePlies= new List<Ply>();
        currentPlyDepth++;
        if(currentPlyDepth > objectivePlyDepth){
            EvaluateBoard(parentPly);
            //Task evaluationTask=Task.Run(()=>EvaluateBoard(parentPly));
            //await evaluationTask;
            return parentPly;
        }
        Ply plyceHolder= new Ply();
        plyceHolder.score= -9999999*minMaxDirection;
        parentPly.bestFuture=plyceHolder;
        foreach(PieceEvaluation eva in team){              
                foreach(AvailableMove move in eva.availableMoves){
                    calculationCount++;
                    Chessboard.instance.selectedPiece=eva.piece;
                    Chessboard.instance.selectedMove=move;
                    TaskCompletionSource<bool> tcs =new TaskCompletionSource<bool>();
                    PieceMovementState.MovePiece(tcs,true,move.moveType);
                    await tcs.Task;
                    Ply newPly=CreateSnapShot(parentPly);
                    newPly.changes=PieceMovementState.changes;
                    newPly.enPassantFlag=PieceMovementState.enPassantFlag;
                    Task<Ply> calculation= CalculatePly(newPly,GetTeam(newPly,minMaxDirection*-1),currentPlyDepth,minMaxDirection*-1);
                    await calculation;
                    parentPly.bestFuture=IsBest(parentPly.bestFuture,minMaxDirection,calculation.Result);
                    newPly.originPly=parentPly;
                    parentPly.futurePlies.Add(newPly);
                    PieceMovementState.enPassantFlag=newPly.enPassantFlag;
                    ResetBoardBackwards(newPly);
                }
            }
            return parentPly.bestFuture;
    }
   List<PieceEvaluation> GetTeam(Ply ply, int minMaxDirection){
        if(minMaxDirection==1){
             return ply.golds;
        }else{
            return ply.greens;
        }
    }
    Ply IsBest(Ply ply,int minMaxDirection,Ply potentialBest){
         if(minMaxDirection==1){
             if(potentialBest.score>ply.score){
                   return potentialBest;
             }
             return ply;
         }else{
             if(potentialBest.score<ply.score){
                   return potentialBest;
             }
             return ply;
         }
    }      
  [ContextMenu("Calculate Plays")]
  public async void CalculatePlays(){
      lastInterval=Time.realtimeSinceStartup;
      int minMaxDirection=1;
      enPassantFlagSaved=PieceMovementState.enPassantFlag;
      currentState=CreateSnapShot();
      calculationCount=0;
      Ply currentPly=currentState;
      currentPly.originPly=null;
      int currentPlyDepth=0;
      currentPly.changes= new List<AffectedPiece>();
      Debug.Log("Começo");
      Task<Ply> calculation= CalculatePly(currentPly,GetTeam(currentPly,minMaxDirection),currentPlyDepth,minMaxDirection);
      await calculation;
      currentPly.bestFuture=calculation.Result;
      Debug.Log("Calculation: "+calculationCount);
      Debug.Log("Time: "+(Time.realtimeSinceStartup-lastInterval));
      PrintBestPly(currentPly.bestFuture);
      PieceMovementState.enPassantFlag=enPassantFlagSaved;
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
  Ply CreateSnapShot(Ply parentPly){
      Ply ply= new Ply();
      ply.golds= new List<PieceEvaluation>();
      ply.greens= new List<PieceEvaluation>();
      foreach(PieceEvaluation p in parentPly.golds){
          if(p.piece.gameObject.activeSelf){
            ply.golds.Add(CreateEvaluationPiece(p.piece,ply));  
          }
      }
      foreach(PieceEvaluation p in parentPly.greens){
          if(p.piece.gameObject.activeSelf){
            ply.greens.Add(CreateEvaluationPiece(p.piece,ply));  
          }
      }
       return ply;
  }
  PieceEvaluation CreateEvaluationPiece(Piece piece, Ply ply){
      PieceEvaluation eva= new PieceEvaluation();
      eva.piece=piece;
      Chessboard.instance.selectedPiece=eva.piece;
      eva.availableMoves=eva.piece.movement.GetValidMoves();
      return eva;
  } 
  
   void EvaluateBoard(Ply ply){
      foreach(PieceEvaluation piece in ply.golds ){
          EvaluatePiece(piece,ply,1);
      }
       foreach(PieceEvaluation piece in ply.greens ){
          EvaluatePiece(piece,ply,-1);
      }
  }
  void EvaluatePiece(PieceEvaluation eva, Ply ply,int scoreDirection){  
      ply.score+=eva.piece.movement.value*scoreDirection;
  }
  void ResetBoardBackwards(Ply ply){
     foreach(AffectedPiece p in ply.changes){
         p.piece.tile.content=null;
         p.piece.tile=p.from;
         p.from.content=p.piece;
         p.piece.wasMoved=p.wasMoved;
        //p.piece.transform.position= new Vector3(p.from.pos.x, p.from.pos.y,0);
         p.piece.gameObject.SetActive(true);
     }
  }
   void PrintBestPly(Ply finalPly){
        Ply currentPly=finalPly;
        Debug.Log("Melhor jogada:");
        while (currentPly.originPly!=null){
            Debug.LogFormat("{0}->{1}->{2}",
            currentPly.changes[0].piece.transform.parent.name,
            currentPly.changes[0].piece.name,
            currentPly.changes[0].to.pos);
            currentPly=currentPly.originPly;
        }
    }
}
