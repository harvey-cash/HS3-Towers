using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    
	void Start () {
        SetToDefaults();
        /* Create GUI and everything */
        BeginNewGame();
	}

    public enum UNIT_TYPES { CUBE, PYRAMID };

    /* SETTINGS */
    private int playerCount, cubeCount, pyramidCount;
    private Vector2 boardDimensions;
    public void SetToDefaults() {
        playerCount = 2;
        cubeCount = 3;
        pyramidCount = 3;
        boardDimensions = new Vector2(7, 7);
    }
    public bool ChangeSettings(int playerCount, Vector2 boardDimensions, int cubeCount, int pyramidCount) {
        this.playerCount = playerCount;
        this.boardDimensions = boardDimensions;
        this.cubeCount = cubeCount;
        this.pyramidCount = pyramidCount;
        return true;
    }    

    /* INITIALISE GAME */
    private bool BeginNewGame() {
        Grid[,] grids = CreateGrids();
        BoardState board = new BoardState(grids, BoardState.PLAYER.ZERO);
        return true;
    }

    private Grid[,] CreateGrids() {
        Grid[,] grids = new Grid[(int)boardDimensions.x, (int)boardDimensions.y];
        for (int x = 0; x < boardDimensions.x; x++) {
            for (int y = 0; x < boardDimensions.y; y++) {
                Grid grid = new Grid(new Vector2(x, y));
                grids[x, y] = grid;
            }
        }

        return new Grid[(int)boardDimensions.x, (int)boardDimensions.y];
    }

    private Unit[] GetNewUnit(UNIT_TYPES type) {
        Unit unit;
        return new Unit[1];
    }
}
