using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class LoadState : State
{
   public override async void Enter()
    {
        Debug.Log("entrou loadState");
        await Chessboard.instance.LoadAsync();
        await LoadAllPiecesAsync();
        machine.currentlyPlaying = machine.player2;
        machine.ChangeTo<TurnBeginState>();
    }

    async Task LoadAllPiecesAsync()
    {
        LoadTeamPieces(Chessboard.instance.greenPieces);
        LoadTeamPieces(Chessboard.instance.goldenPieces);
        await Task.Delay(100);
    }

    void LoadTeamPieces(List<Piece> pieces)
    {
        foreach (Piece piece in pieces)
        {
            Chessboard.instance.AddPiece(piece.transform.parent.name, piece);
        }
    }

}
