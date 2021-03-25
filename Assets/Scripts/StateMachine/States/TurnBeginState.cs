using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class TurnBeginState : State
{
    public override async void Enter()
    {
        Debug.Log("entrou turn begin");
        if (machine.currentlyPlaying == machine.player1)
            machine.currentlyPlaying = machine.player2;
        else
            machine.currentlyPlaying = machine.player1;
        Debug.Log(machine.currentlyPlaying + " jogando ");
        await Task.Delay(100);
        machine.ChangeTo<TurnEndState>();
    }
}
