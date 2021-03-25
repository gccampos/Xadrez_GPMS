using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
      
public abstract class Piece : MonoBehaviour
{
  public Tile tile;
   void OnMouseDown(){
         Chessboard.instance.tileClicked(this, transform.parent.GetComponent<Player>());
   }
   
   void OnMouseUp(){
          Debug.Log("Deixou de clicar em "+transform);
   } 
}
