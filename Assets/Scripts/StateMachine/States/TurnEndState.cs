using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class TurnEndState : State
{
   public override async void Enter()
    {
        Debug.Log("TurnEndState");
        bool gameFinished = CheckTeams();
        await Task.Delay(100);
        if (gameFinished)
            machine.ChangeTo<GameEndState>();
        else
            machine.ChangeTo<TurnBeginState>();
    }

    bool CheckTeams()
    {
        Piece goldPiece = Chessboard.instance.GoldenPieces.Find((x) => x.gameObject.activeSelf == true);
        if (goldPiece == null)
        {
            Debug.Log("Lado verde ganhou");
            return true;
        }
        Piece greenPiece = Chessboard.instance.GreenPieces.Find((x) => x.gameObject.activeSelf == true);
        if (greenPiece == null)
        {
            Debug.Log("Lado dourado ganhou");
            return true;
        }
        return false;
    }
}
