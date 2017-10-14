using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    private bool debug = true;
    GameObject unitFolder;

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
        boardDimensions = new Vector2(7, 9);
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
        unitFolder = new GameObject("Unit Folder");
        Grid[,] grids = CreateGrids();
        grids = PopulateWithUnits(grids);

        BoardState board = new BoardState(grids, Player.TEAM.ZERO);

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
                GameObject tile = (GameObject)Instantiate(Resources.Load("Grid"));
                Grid grid = tile.AddComponent<Grid>();
                grid.InitGrid(new Vector2(x, y));

                tile.transform.parent = unitFolder.transform;
                tile.transform.position = new Vector3(x, -(0.5f * tile.transform.localScale.y), y);
                grids[x, y] = grid;
            }
        }

        return grids;
    }

    private Grid[,] PopulateWithUnits(Grid[,] grids) {
        int otherSide = grids.GetLength(1) - 1;
        int end = grids.GetLength(0) - 1;

        Unit[] occupants;
        for (int x = 1; x < end; x++) {
            occupants = new Unit[1];
            occupants[0] = NewUnit(Unit.UNIT_TYPES.CUBE, Player.TEAM.ZERO, new Vector2(x, 0));
            grids[x, 0].SetOccupants(occupants);

            occupants = new Unit[1];
            occupants[0] = NewUnit(Unit.UNIT_TYPES.PYRAMID, Player.TEAM.ZERO, new Vector2(x, 1));
            grids[x, 1].SetOccupants(occupants);

            occupants = new Unit[1];
            occupants[0] = NewUnit(Unit.UNIT_TYPES.PYRAMID, Player.TEAM.ONE, new Vector2(x, otherSide - 1));
            grids[x, otherSide - 1].SetOccupants(occupants);

            occupants = new Unit[1];
            occupants[0] = NewUnit(Unit.UNIT_TYPES.CUBE, Player.TEAM.ONE, new Vector2(x, otherSide));
            grids[x, otherSide].SetOccupants(occupants);
        }


        return grids;
    }

    private Unit NewUnit(Unit.UNIT_TYPES type, Player.TEAM team, Vector2 coords) {
        GameObject unitObject;
        Unit unit;
        switch (type) {
            case Unit.UNIT_TYPES.CUBE:
                unitObject = (GameObject)Instantiate(Resources.Load("Cube"));
                unit = unitObject.AddComponent<Cube>();
                break;
            case Unit.UNIT_TYPES.PYRAMID:
                unitObject = (GameObject)Instantiate(Resources.Load("Pyramid"));
                unit = unitObject.AddComponent<Pyramid>();
                break;

            default:
                unitObject = (GameObject)Instantiate(Resources.Load("Cube"));
                unit = unitObject.AddComponent<Cube>();
                break;
        }
        unit.SetTeam(team);
        unitObject.transform.parent = unitFolder.transform;
        unitObject.transform.position = new Vector3(coords.x, (0.5f * unitObject.transform.localScale.y), coords.y);
        unitObject.GetComponent<Renderer>().material.color = Player.TeamColour(team, false);
        
        return unit;
    }
}
