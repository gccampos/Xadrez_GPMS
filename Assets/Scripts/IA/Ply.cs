using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ply
{
    public List<PieceEvaluation> golds;
    public List<PieceEvaluation> greens;
    public int score;
    public List<AffectedPiece> changes;
    public Ply originPly;
    public AvailableMove enPassantFlag;
    public List<Ply> futurePlies;
    public Ply bestFuture;
}
