using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
  public Tile tile;
     
   void OnMouseDown(){
          Debug.Log("Clicou em "+transform);
          Debug.Log("time " + transform.parent.name);
   }
   void OnMouseUp(){
          Debug.Log("Deixou de clicar em "+transform);
   }
   
}
