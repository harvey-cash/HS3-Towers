using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardState : MonoBehaviour {

    private Grid[,] board;
    public enum PLAYER { ZERO, ONE };
    private PLAYER turnOf;

    public BoardState(Grid[,] board, PLAYER turnOf) {
        this.board = board;
        this.turnOf = turnOf;
    }
}
