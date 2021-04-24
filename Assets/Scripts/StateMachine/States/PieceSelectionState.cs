using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PieceSelectionState : State
{
     void PieceClicked(object sender, object args)
    {     
        Piece piece = sender as Piece;
        Player player = args as Player;
        if (machine.currentlyPlaying == player)
        {
            Debug.Log(piece + " fora clicada");
            Chessboard.instance.selectedPiece = piece;
            machine.ChangeTo<MoveSelectionState>();
        }
    }
    public override void Enter()
    {
        InputController.instance.tileClicked += PieceClicked;
        SetColliders(true);
    }

    public override void Exit()
    {
        InputController.instance.tileClicked -= PieceClicked;
        SetColliders(false);
    }
    void SetColliders(bool state){
        foreach(BoxCollider2D b in machine.currentlyPlaying.GetComponentsInChildren<BoxCollider2D>()){
            b.enabled=state;
        }
    }
   
}