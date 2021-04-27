﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnMovement : Movement
{
    Vector2Int direction;
    public PawnMovement(Vector2Int rcvDirection){
        direction=rcvDirection;
        value = 100;
    }
    public override List<AvailableMove> GetValidMoves()
    {
        List<AvailableMove> moveable = GetPawnAttack(direction);
        List<AvailableMove> moves;
        if (!Chessboard.instance.selectedPiece.wasMoved)
        {
            moves = UntilBlockedPath(direction, false, 2);
            if (moves.Count == 2)
            {
                moves[1] = new AvailableMove(moves[1].pos, MoveType.PawnDoubleMove);
            }
        }
        else
        {
            moves = UntilBlockedPath(direction, false, 1);
            if(moves.Count>0){
                moves[0]=CheckPromotion(moves[0]);
            }
        }
        moveable.AddRange(moves);
        return moveable;
    }
    List<AvailableMove> GetPawnAttack(Vector2Int direction)
    {
        List<AvailableMove> pawnAttack = new List<AvailableMove>();
        Piece piece = Chessboard.instance.selectedPiece;
        Vector2Int leftPos = new Vector2Int(piece.tile.pos.x - 1, piece.tile.pos.y + direction.y);
        Vector2Int rightPos = new Vector2Int(piece.tile.pos.x + 1, piece.tile.pos.y + direction.y);
        GetPawnAttack(GetTile(leftPos), pawnAttack);
        GetPawnAttack(GetTile(rightPos), pawnAttack);
        return pawnAttack;
    }
    void GetPawnAttack(Tile tile, List<AvailableMove> pawnAttack)
    {
        if (tile == null)
        {
            return;
        }
        if (isEnemy(tile))
        {
            pawnAttack.Add(new AvailableMove(tile.pos, MoveType.Normal));
        }
        else if (PieceMovementState.enPassantFlag.moveType==MoveType.EnPassant && PieceMovementState.enPassantFlag.pos==tile.pos)
        {
            pawnAttack.Add(new AvailableMove(tile.pos, MoveType.EnPassant));
        }
    }
    AvailableMove CheckPromotion(AvailableMove availableMove)
    {
      int promotionHeight=0;
      if(Chessboard.instance.selectedPiece.maxTeam){
          promotionHeight=7;
      }
      if(availableMove.pos.y != promotionHeight){
          return availableMove;
      }
      return new AvailableMove(availableMove.pos, MoveType.Promotion);
    }
}
