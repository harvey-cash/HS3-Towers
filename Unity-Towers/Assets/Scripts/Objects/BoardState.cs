using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardState {

    private Grid[,] board; 
    public Grid[,] GetGrids() { return board; }

    private Unit selectedUnit;
    public Unit GetSelectedUnit() { return selectedUnit; } 

    private Player.TEAM turnOf;
    public Player.TEAM GetTurnOf() { return turnOf; }

    public BoardState(Grid[,] board, Player.TEAM turnOf, Unit selectedUnit) {
        this.board = board;
        this.turnOf = turnOf;
        this.selectedUnit = selectedUnit;
    }

    public override string ToString() {
        return board.ToString() + " currently turn of: " + turnOf.ToString();
    }
}
