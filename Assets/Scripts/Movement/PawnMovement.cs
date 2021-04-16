using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnMovement : Movement
{
    public override List<Tile> GetValidMoves()
    {
        Vector2Int direction = GetDirection();
        int limit = 1;
        if(!Chessboard.instance.selectedPiece.wasMoved){
            limit = 2;
        }
        List<Tile> moveable = UntilBlockedPath(direction, false, limit);
        moveable.AddRange(GetPawnAttack(direction));
        SetNormalMove(moveable);
        return moveable;
    }

    Vector2Int GetDirection()
    {
        if (StateMachineController.instance.currentlyPlaying.transform.name == "GreenPieces")
            return new Vector2Int(0, -1);
        return new Vector2Int(0, 1);
    }

    List<Tile> GetPawnAttack(Vector2Int direction){
        List<Tile> pawnAttack=new  List<Tile>();
        Tile temp;
        Piece piece= Chessboard.instance.selectedPiece;
        Vector2Int leftPos=new Vector2Int(piece.tile.pos.x-1,piece.tile.pos.y+ direction.y);
        Vector2Int rightPos=new Vector2Int(piece.tile.pos.x+1,piece.tile.pos.y+ direction.y);
        temp = GetTile(leftPos);
        if(temp != null && isEnemy(temp)){
            pawnAttack.Add(temp);
        }
        temp = GetTile(rightPos);
        if(temp != null && isEnemy(temp)){
            pawnAttack.Add(temp);
        }
        return pawnAttack;
    }
}
