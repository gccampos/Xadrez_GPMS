using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement
{
    public abstract List<Tile> GetValidMoves();

    protected bool isEnemy(Tile tile){
        if(tile.content != null && tile.content.transform.parent != Chessboard.instance.selectedPiece.transform.parent){
            return true;
        }
        return false;      
    }

    protected Tile GetTile(Vector2Int position){
        Tile tile;
        Chessboard.instance.tiles.TryGetValue(position, out tile);
        return tile;
    }

    protected List<Tile> UntilBlockedPath(Vector2Int direction, bool includeBlocked, int limit){
        List<Tile> moves = new List<Tile>();
        Tile current = Chessboard.instance.selectedPiece.tile;
        while(current != null && moves.Count<limit>){
            if(Chessboard.instance.tiles.TryGetValue(current.pos+direction, out current)){
                if(current.content == null){
                    moves.Add(current);
                }else if(isEnemy(current)){
                    if(includeBlocked)
                        moves.Add(current);
                    return moves;
                }else{ //era um aliado
                    return moves;
                }
            }
        }
        return moves;
    }
}
