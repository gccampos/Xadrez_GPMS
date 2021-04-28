using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AIPlayingState : State
{
   public async override void Enter(){
       Task<Ply> task= AiController.instance.CalculatePlays();
       await task;
       Ply bestResult=task.Result;
       MakeBestPlay(bestResult);
   }
   async void MakeBestPlay(Ply ply){
       Ply currentPly=ply;
       for(int i=1; i<AiController.instance.objectivePlyDepth;i++){
           currentPly=currentPly.originPly;
       }
       Chessboard.instance.selectedPiece=currentPly.changes[0].piece;
       Chessboard.instance.selectedMove=GetMoveType(currentPly);
       await Task.Delay(100);
       machine.ChangeTo<PieceMovementState>();
   }
   AvailableMove GetMoveType(Ply ply){
       List<PieceEvaluation> team;
       if(machine.currentlyPlaying==machine.player1){
           team=ply.golds;
       }else{
           team=ply.greens;
       }
       List<AvailableMove> moves=Chessboard.instance.selectedPiece.movement.GetValidMoves();
       foreach(AvailableMove m in moves){
           if(m.pos== ply.changes[0].to.pos){
               return m;
           }
       }
       return new AvailableMove();
   }
}
