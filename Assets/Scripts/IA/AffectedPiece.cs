﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffectedPiece
{
    
   public Piece piece;
   public Tile from;
   public Tile to;
   public virtual void Undo(){
      piece.tile.content=null;
      piece.tile=from;
      from.content=piece;
      piece.gameObject.SetActive(true);
   }
}
public class AffectedKingRook:AffectedPiece
{
public bool wasMoved;
public override void Undo(){
      base.Undo();
      piece.wasMoved=wasMoved;
}

}
public class AffectedPawn:AffectedPiece
{
public bool resetMovement;
public bool wasMoved;

public  override void Undo(){
      base.Undo();
      piece.wasMoved=wasMoved;
      if(resetMovement){
         Pawn pawn=piece as Pawn;
         piece.movement=pawn.savedMovement;
      }
}

}
