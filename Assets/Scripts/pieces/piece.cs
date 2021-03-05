using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class piece : MonoBehaviour
{
  public tile tile;
     
   void OnMouseDown(){
          Debug.Log("Clicou em "+transform);
   }
   void OnMouseUp(){
          Debug.Log("Deixou de clicar em "+transform);
   }
}
