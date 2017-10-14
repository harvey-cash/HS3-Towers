using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardState {

    private Grid[,] board; 
    public Grid[,] GetBoard() { return board; }   

    private Player.TEAM turnOf;

    public BoardState(Grid[,] board, Player.TEAM turnOf) {
        this.board = board;
        this.turnOf = turnOf;
    }

    public override string ToString() {
        return board.ToString() + " currently turn of: " + turnOf.ToString();
    }
}
