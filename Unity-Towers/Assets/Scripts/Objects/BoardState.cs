using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardState : MonoBehaviour {

    private Grid[,] board;    
    private Player.TEAM turnOf;

    public BoardState(Grid[,] board, Player.TEAM turnOf) {
        this.board = board;
        this.turnOf = turnOf;
    }
}
