﻿using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
  public static AiController instance;
  public Ply currentState;
  int calculationCount;
  public int objectivePlyDepth=2;
  public HighlightClick AIhighlight;
  void Awake(){
      instance=this;
  }
    async Task<Ply> CalculatePly(Ply parentPly,List<PieceEvaluation> team, int currentPlyDepth, int minMaxDirection){
        parentPly.futurePlies= new List<Ply>();
        currentPlyDepth++;
        if(currentPlyDepth > objectivePlyDepth){
            EvaluateBoard(parentPly);
            return parentPly;
        }
        Ply plyceHolder= new Ply();
        plyceHolder.score= -99999*minMaxDirection;
        parentPly.bestFuture=plyceHolder;
        foreach(PieceEvaluation eva in team){              
                foreach(Tile t in eva.availableMoves){
                    calculationCount++;
                    Chessboard.instance.selectedPiece=eva.piece;
                    Chessboard.instance.selectedHighlight=AIhighlight;
                    AIhighlight.tile=t;
                    AIhighlight.transform.position= new Vector3(t.pos.x,t.pos.y,0);
                    TaskCompletionSource<bool> tcs =new TaskCompletionSource<bool>();
                    PieceMovementState.MovePiece(tcs,true);
                    await tcs.Task;
                    Ply newPly=CreateSnapShot();
                    newPly.name=string.Format("{0}, {1} to {2}", parentPly.name,eva.piece.transform.parent.name+eva.piece.name,t.pos);
                    newPly.changes=PieceMovementState.changes;
                    Task<Ply> calculation= CalculatePly(newPly,GetTeam(newPly,minMaxDirection*-1),currentPlyDepth,minMaxDirection*-1);
                    await calculation;
                    parentPly.bestFuture=IsBest(parentPly.bestFuture,minMaxDirection,calculation.Result);
                    newPly.moveType=t.moveType;
                    newPly.originPly=parentPly;
                    parentPly.futurePlies.Add(newPly);
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
      int minMaxDirection=1;
      currentState=CreateSnapShot();
      currentState.name="start";
      calculationCount=0;
      Ply currentPly=currentState;
      currentPly.originPly=null;
      int currentPlyDepth=0;
      currentPly.changes= new List<AffectedPiece>();
      Debug.Log("Começo");
      Task<Ply> calculation= CalculatePly(currentPly,GetTeam(currentPly,minMaxDirection),currentPlyDepth,minMaxDirection);
      await calculation;
      currentPly.bestFuture=calculation.Result;
      Debug.LogFormat("Melhor jogada para o dourado: {0}, com score: {1}", currentPly.bestFuture.name, currentPly.bestFuture.score);
      Debug.Log("Calculation: "+calculationCount);
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
      eva.score=eva.piece.movement.value;
      ply.score+=eva.score*scoreDirection;
  }
  void ResetBoardBackwards(Ply ply){
     foreach(AffectedPiece p in ply.changes){
         p.piece.tile.content=null;
         p.piece.tile=p.from;
         p.from.content=p.piece;
         p.piece.transform.position= new Vector3(p.from.pos.x, p.from.pos.y,0);
         p.piece.gameObject.SetActive(true);
     }
  }
}