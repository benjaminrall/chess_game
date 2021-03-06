﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{

    public override bool checkIsValidMove(int attemptedX, int attemptedY)
    {
        if (availableSpaces.Contains((attemptedX, attemptedY))){
            if (PieceAt(attemptedX, attemptedY)){
                TakePieceAt(attemptedX, attemptedY, colour);
            }
            return true;
        }
        return false;
    }

    public override void FindAvailableSpaces(){
        base.FindAvailableSpaces();
        CheckSpaces(1, 7 - pieceY, 0, 1);
        CheckSpaces(1, pieceY, 0, -1);
        CheckSpaces(1, 7 - pieceX, 1, 0);
        CheckSpaces(1, pieceX, -1, 0);
        CheckSpaces(1, Math.Min(7 - pieceX, 7 - pieceY), 1, 1);
        CheckSpaces(1, Math.Min(7 - pieceX, pieceY), 1, -1);
        CheckSpaces(1, Math.Min(pieceX, 7 - pieceY), -1, 1);
        CheckSpaces(1, Math.Min(pieceX, pieceY), -1, -1);
    }

    public override void FindTempSpaces(){
        base.FindTempSpaces();
        CheckSpaces(1, 7 - pieceY, 0, 1, true);
        CheckSpaces(1, pieceY, 0, -1, true);
        CheckSpaces(1, 7 - pieceX, 1, 0, true);
        CheckSpaces(1, pieceX, -1, 0, true);
        CheckSpaces(1, Math.Min(7 - pieceX, 7 - pieceY), 1, 1, true);
        CheckSpaces(1, Math.Min(7 - pieceX, pieceY), 1, -1, true);
        CheckSpaces(1, Math.Min(pieceX, 7 - pieceY), -1, 1, true);
        CheckSpaces(1, Math.Min(pieceX, pieceY), -1, -1, true);
    }   
}
