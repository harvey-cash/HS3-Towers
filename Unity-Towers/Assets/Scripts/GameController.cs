using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    private bool debug = true;
    
	void Start () {
        SetToDefaults();
        /* Create GUI and everything */
        BeginNewGame();        
	}

    /* ~~~~~~~~~~~~~~~~~~~~~ SETTINGS ~~~~~~~~~~~~~~~~~~~~~ */
    private List<Player.PLAYER_TYPES> playerTypes = new List<Player.PLAYER_TYPES>();
    private int cubeCount, pyramidCount;
    private Vector2 boardDimensions;
    public void SetToDefaults() {
        playerTypes.Add(Player.PLAYER_TYPES.HUMAN);
        playerTypes.Add(Player.PLAYER_TYPES.HUMAN);

        cubeCount = 3;
        pyramidCount = 3;
        boardDimensions = new Vector2(7, 7);
    }
    public bool ChangeSettings(List<Player.PLAYER_TYPES> playerTypes, Vector2 boardDimensions, int cubeCount, int pyramidCount) {
        this.playerTypes = playerTypes;
        this.boardDimensions = boardDimensions;
        this.cubeCount = cubeCount;
        this.pyramidCount = pyramidCount;
        return true;
    }


    /* ~~~~~~~~~~~~~~~~~~~~~ INITIALISE GAME ~~~~~~~~~~~~~~~~~~~~~ */
    private bool BeginNewGame() {
        if (debug) { Debug.Log("Starting New Game"); }

        Player[] players = CreatePlayers();
        if (debug) { Debug.Log(players.ToString()); }

        Grid[,] grids = CreateGrids();
        if (debug) { Debug.Log(grids.ToString()); }

        BoardState board = new BoardState(grids, Player.TEAM.ZERO);
        if (debug) { Debug.Log(board.ToString()); }

        ObjectController animator = new ObjectController(board);
        animator.DrawGrids();

        return true;
    }

    private Player[] CreatePlayers() {
        Player[] players = new Player[playerTypes.Count];
        for (int i = 0; i < playerTypes.Count; i++) {
            players[i] = new HumanPlayer(i);
        }
        return players;
    }

    private Grid[,] CreateGrids() {
        Grid[,] grids = new Grid[(int)boardDimensions.x, (int)boardDimensions.y];
        for (int x = 0; x < boardDimensions.x; x++) {
            for (int y = 0; y < boardDimensions.y; y++) {
                Grid grid = new Grid(new Vector2(x, y));
                grids[x, y] = grid;
            }
        }

        return grids;
    }

    private Unit[] NewUnitArray(Unit.UNIT_TYPES type, Player.TEAM team) {
        Unit unit = Unit.NewUnit(type, team);
        Unit[] unitArray = new Unit[1];
        unitArray[0] = unit;
        return unitArray;
    }
}
