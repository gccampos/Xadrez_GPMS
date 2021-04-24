﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
  public static AiController instance;
  public Ply currentState;
  void Awake(){
      instance=this;
  }
  [ContextMenu("Create Evaluations")]
  public void CreateEvaluations(){
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
       currentState=ply;
  }
  PieceEvaluation CreateEvaluationPiece(Piece piece, Ply ply){
      PieceEvaluation eva= new PieceEvaluation();
      eva.piece=piece;
      return eva;
  } 
  [ContextMenu("Evaluate")]
  public void EvaluateBoard(){
      Ply ply=currentState;
      foreach(PieceEvaluation piece in ply.golds ){
          EvaluatePiece(piece,ply,1);
      }
       foreach(PieceEvaluation piece in ply.greens ){
          EvaluatePiece(piece,ply,-1);
      }
      Debug.Log(ply.score);
  }
  void EvaluatePiece(PieceEvaluation eva, Ply ply,int scoreDirection){
      Chessboard.instance.selectedPiece=eva.piece;
      List<Tile> tiles=eva.piece.movement.GetValidMoves();
      eva.availableMoves=tiles.Count;
      eva.score=eva.piece.movement.value;
      ply.score+=eva.score*scoreDirection;
  }
}
