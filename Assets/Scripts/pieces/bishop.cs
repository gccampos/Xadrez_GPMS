﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Bishop : Piece
{
    void Awake(){
        movement = new BishopMovement();
    }
}
