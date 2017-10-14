using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    private bool debug = true;
    GameObject unitFolder;
    BoardState board;

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
    private void BeginNewGame() {
        if (debug) { Debug.Log("Starting New Game"); }

        Player[] players = CreatePlayers();
        unitFolder = new GameObject("Unit Folder");
        Grid[,] grids = CreateGrids();
        grids = PopulateWithUnits(grids);

        board = new BoardState(grids, Player.TEAM.ZERO, null);

        PlayGame(board, players);
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
                grid.SetController(this);

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
            occupants[0].SetGrid(grids[x, 0]);
            grids[x, 0].SetOccupants(occupants);

            occupants = new Unit[1];
            occupants[0] = NewUnit(Unit.UNIT_TYPES.PYRAMID, Player.TEAM.ZERO, new Vector2(x, 1));
            occupants[0].SetGrid(grids[x, 1]);
            grids[x, 1].SetOccupants(occupants);

            occupants = new Unit[1];
            occupants[0] = NewUnit(Unit.UNIT_TYPES.PYRAMID, Player.TEAM.ONE, new Vector2(x, otherSide - 1));
            occupants[0].SetGrid(grids[x, otherSide - 1]);
            grids[x, otherSide - 1].SetOccupants(occupants);

            occupants = new Unit[1];
            occupants[0] = NewUnit(Unit.UNIT_TYPES.CUBE, Player.TEAM.ONE, new Vector2(x, otherSide));
            occupants[0].SetGrid(grids[x, otherSide]);
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
        unit.SetController(this);

        unitObject.transform.parent = unitFolder.transform;
        unitObject.transform.position = new Vector3(coords.x, (0.5f * unitObject.transform.localScale.y), coords.y);
        unitObject.GetComponent<Renderer>().material.color = Player.TeamColour(team, false);
        
        return unit;
    }


    /* ~~~~~~~~~~~~~~~~~~~~~ PLAY GAME ~~~~~~~~~~~~~~~~~~~~~ */
    private void PlayGame(BoardState board, Player[] players) {
        
    }

    //ONLY PASS ALONG THE ACTIVE UNIT IN THE SELECTED GRID
    public void OnUnitClick(Unit unit) {
        if (board.GetSelectedUnit() == null) {
            AttemptMove(unit.GetGrid().GetActiveUnit(), unit.GetGrid());
        } else {
            AttemptMove(board.GetSelectedUnit(), unit.GetGrid());
        }
    }
    public void OnGridClick(Grid grid) {
        if (board.GetSelectedUnit() != null) {
            AttemptMove(board.GetSelectedUnit(), grid);
        }
    }
        
    private void AttemptMove(Unit selectedUnit, Grid targetGrid) {
        //SELECTING A UNIT
        if (board.GetSelectedUnit() == null) {
            if (debug) { Debug.Log("Selected unit: " + selectedUnit.GetGrid().GetCoords().ToString()); }
            board = new BoardState(board.GetGrids(), board.GetTurnOf(), selectedUnit);
        }

        //ATTEMPTING TO MOVE TO THE CURRENT GRID
        else if (selectedUnit.GetGrid().Equals(targetGrid)) {
            if (debug) { Debug.Log("You can't move to the same grid."); }
        }

        //CAN MOVE INTO EMPTY GRIDS        
        else if (targetGrid.GetActiveUnit() == null) {
            MoveUnitToGrid(board.GetGrids(), selectedUnit, targetGrid);
        }

        //MOVING ONTO A FRIENDLY
        else if (selectedUnit.GetTeam().Equals(targetGrid.GetActiveUnit().GetTeam())) {
            if (targetGrid.GetActiveUnit().GetComponent<Pyramid>() != null) {
                if (debug) { Debug.Log("You can't move onto a friendly pyramid"); }
            } else {
                MoveUnitToGrid(board.GetGrids(), selectedUnit, targetGrid);
            }
        }

        //MOVING ONTO AN ENEMY
        else {
            MoveUnitToGrid(board.GetGrids(), selectedUnit, targetGrid);
        }
    }

    private void MoveUnitToGrid(Grid[,] gridArray, Unit selectedUnit, Grid targetGrid) {
        if(!ValidMove(targetGrid)) {
            if (debug) { Debug.Log("Move is invalid!"); }
            return;
        }

        gridArray = MoveOffGrid(gridArray, selectedUnit, targetGrid);

        /* ~~~~~~~ STACK / MOVE TO EMPTY ~~~~~~~~ */
        if (targetGrid.GetActiveUnit() == null || targetGrid.GetActiveUnit().GetTeam().Equals(selectedUnit.GetTeam())) {
            
            Unit[] moreOccupants = new Unit[targetGrid.GetOccupants().Length + 1];
            for (int i = 0; i < targetGrid.GetOccupants().Length; i++) {
                moreOccupants[i] = targetGrid.GetOccupants()[i];
            }
            moreOccupants[moreOccupants.Length - 1] = selectedUnit;

            selectedUnit.transform.position = new Vector3(targetGrid.GetCoords().x, targetGrid.GetOccupants().Length + (0.5f * selectedUnit.transform.localScale.y), targetGrid.GetCoords().y);

            targetGrid.SetOccupants(moreOccupants);
            selectedUnit.SetGrid(targetGrid);

            board = new BoardState(gridArray, board.GetTurnOf(), null);
        }

        /* ~~~~~~~ TAKE (REPLACE) ~~~~~~~~ */
        else {
            Unit[] moreOccupants = new Unit[targetGrid.GetOccupants().Length];
            for (int i = 0; i < targetGrid.GetOccupants().Length; i++) {
                moreOccupants[i] = targetGrid.GetOccupants()[i];
            }
            Unit takenUnit = moreOccupants[moreOccupants.Length - 1];
            moreOccupants[moreOccupants.Length - 1] = selectedUnit;
            if(debug) { Debug.Log("Player " + selectedUnit.GetTeam().ToString() + " has taken a piece"); }
            Destroy(takenUnit.gameObject);

            selectedUnit.transform.position = new Vector3(targetGrid.GetCoords().x, targetGrid.GetOccupants().Length + (0.5f * selectedUnit.transform.localScale.y) - 1, targetGrid.GetCoords().y);

            targetGrid.SetOccupants(moreOccupants);
            selectedUnit.SetGrid(targetGrid);

            board = new BoardState(gridArray, board.GetTurnOf(), null);
        }

        
    }

    private Grid[,] MoveOffGrid(Grid[,] gridArray, Unit selectedUnit, Grid targetGrid) {
        /* REMOVE UNIT FROM GRID */
        Unit[] fewerOccupants = new Unit[selectedUnit.GetGrid().GetOccupants().Length - 1];
        for (int i = 0; i < selectedUnit.GetGrid().GetOccupants().Length - 1; i++) {
            fewerOccupants[i] = selectedUnit.GetGrid().GetOccupants()[i];
        }
        selectedUnit.GetGrid().SetOccupants(fewerOccupants);
        return gridArray;
    }

    public bool ValidMove(Grid targetGrid) {
        if (board.GetSelectedUnit() == null) {
            return false;
        }
        else {
            Grid originGrid = board.GetSelectedUnit().GetGrid();
            Vector2 move = targetGrid.GetCoords() - originGrid.GetCoords();
            int distance = Mathf.RoundToInt(Mathf.Sqrt(Vector2.SqrMagnitude(move)));

            if(board.GetSelectedUnit().GetComponent<Cube>() != null && distance > 1) {
                return false;
            }

            //For moves parallel to the edges
            if ((move.x == 0 || move.y == 0) && distance <= originGrid.GetOccupants().Length) {
                return true;
            }
            //For moves diagonal
            else if ((Mathf.Abs(move.x) == Mathf.Abs(move.y)) && distance <= Mathf.Abs(move.x)) {
                return true;
            } 
            
            else {
                return false;
            }

        }
    }
}
