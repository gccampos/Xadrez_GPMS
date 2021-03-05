using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chessboard : MonoBehaviour
{
   public static chessboard instance;
   public Dictionary<Vector2Int, tile> tiles= new Dictionary<Vector2Int, tile>();
   public List<piece> goldenPieces=new List<piece>();
   public List<piece> greenPieces=new List<piece>();
   void Awake(){
       instance=this;
       CreateBoard();
   }
   void CreateBoard(){
       for(int i=0;i<8;i++){
           for(int j=0;j<8;j++){
              CreateTile(i,j);
           }
       }
   }

   void CreateTile(int i,int j){
      tile tile=new tile();
      tile.pos=new Vector2Int(i,j);
      tiles.Add(tile.pos,tile);
   }


}