using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class TurnEndState : State
{
   public override async void Enter()
    {
        Debug.Log("entrou TurnEndState");
        bool gameFinished = CheckConditions();
        await Task.Delay(100);
        if (gameFinished)
            machine.ChangeTo<GameEndState>();
        else
            machine.ChangeTo<TurnBeginState>();
    }

    bool CheckConditions(){
        if(CheckTeams() || CheckKing()){
            return true;
        }
        return false;
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
    
    bool CheckKing(){
        King king = Chessboard.instance.goldHolder.GetComponentInChildren<King>();
        if(king == null){
            Debug.Log("Lado Verde Ganhou");
            return true;
        }
        king = Chessboard.instance.greenHolder.GetComponentInChildren<King>();
        if(king == null){
            Debug.Log("Lado Dourado Ganhou");
            return true;
        }
        return false;
    }
}
