using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    private bool debug = true;
    GameObject unitFolder;
    BoardState board;

    private List<Cube> movedCubes;
    private List<Pyramid> movedPyramids;
    private bool canPass = false;
    public bool GetCanPass() { return canPass; }

    private CameraController cameraController;

    Player[] players;

    void Start () {
        SetToDefaults();
        cameraController = (Instantiate(Resources.Load("CameraRig")) as GameObject).GetComponent<CameraController>();
        cameraController.transform.position = new Vector3((0.5f * boardDimensions.x) - 0.5f, 0, (0.5f * boardDimensions.y) - 0.5f);
        /* Create GUI and everything */
        BeginNewGame();        
	}

    /* ~~~~~~~~~~~~~~~~~~~~~ SETTINGS ~~~~~~~~~~~~~~~~~~~~~ */
    private List<Player.PLAYER_TYPES> playerTypes = new List<Player.PLAYER_TYPES>();
    private int cubeCount, pyramidCount;
    private int cubeTurns, pyramidTurns;
    private Vector2 boardDimensions;
    public void SetToDefaults() {
        playerTypes.Add(Player.PLAYER_TYPES.AI);
        playerTypes.Add(Player.PLAYER_TYPES.HUMAN);

        cubeCount = 3;
        pyramidCount = 3;
        cubeTurns = 1;
        pyramidTurns = 1;

        boardDimensions = new Vector2(5, 5);
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

        players = CreatePlayers();
        unitFolder = new GameObject("Unit Folder");
        Grid[,] grids = CreateGrids();
        grids = PopulateWithUnits(grids);

        board = new BoardState(grids, Player.TEAM.ZERO, null);
        movedCubes = new List<Cube>();
        movedPyramids = new List<Pyramid>();

        PlayGame(board, players);
    }

    private Player[] CreatePlayers() {
        Player[] players = new Player[playerTypes.Count];
        for (int i = 0; i < playerTypes.Count; i++) {
            switch(playerTypes[i]) {
                case Player.PLAYER_TYPES.HUMAN: players[i] = new HumanPlayer(i); break;
                case Player.PLAYER_TYPES.AI: players[i] = new AIPlayer(i); break;

                default: players[i] = new AIPlayer(i); break;
            }
            players[i].SetGameController(this);
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
        if(players[0].GetPlayerType().Equals(Player.PLAYER_TYPES.AI)) {
            ((AIPlayer)players[0]).MakeMove(board);
        }
    }
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.E) && canPass) {
            Debug.Log("RELEASE TO PASS TURN.");
        }
        if (Input.GetKeyUp(KeyCode.E) && canPass) {
            PassTurn();
        }
    } 

    //ONLY PASS ALONG THE ACTIVE UNIT IN THE SELECTED GRID
    public bool OnUnitClick(Unit unit) {
        if (board.GetSelectedUnit() == null) {
            return AttemptMove(unit.GetGrid().GetActiveUnit(board.GetTurnOf()), unit.GetGrid());
        } else {
            return AttemptMove(board.GetSelectedUnit(), unit.GetGrid());
        }
    }
    public bool OnGridClick(Grid grid) {
        if (board.GetSelectedUnit() != null) {
            return AttemptMove(board.GetSelectedUnit(), grid);
        } else {
            return false;
        }
    }
        
    private bool AttemptMove(Unit selectedUnit, Grid targetGrid) {
        //SELECTING A UNIT
        if (board.GetSelectedUnit() == null) {
            if (selectedUnit == null) {
                if(debug) { Debug.Log("You cannot move your opponent's pieces. Obviously."); }
                return false;
            }
            if (selectedUnit.GetTeam().Equals(board.GetTurnOf())) {
                if (debug) { Debug.Log("Selected unit: " + selectedUnit.GetGrid().GetCoords().ToString()); }
                board = new BoardState(board.GetGrids(), board.GetTurnOf(), selectedUnit);
                return true;
            }
            else {
                if (debug) { Debug.Log("It is currently the turn of player " + board.GetTurnOf().ToString()); }
                return false;
            }            
        }

        //ATTEMPTING TO MOVE TO THE CURRENT GRID
        else if (selectedUnit.GetGrid().Equals(targetGrid)) {
            if (debug) { Debug.Log("You can't move to the same grid."); }
            return false;
        }

        //CAN MOVE INTO EMPTY GRIDS        
        else if (targetGrid.GetActiveUnit() == null) {
            return MoveUnitToGrid(board.GetGrids(), selectedUnit, targetGrid);
        }

        //MOVING ONTO A FRIENDLY
        else if (selectedUnit.GetTeam().Equals(targetGrid.GetActiveUnit().GetTeam())) {
            if (targetGrid.GetActiveUnit().GetComponent<Pyramid>() != null) {
                if (debug) { Debug.Log("You can't move onto a friendly pyramid"); }
                return false;
            } else {
                return MoveUnitToGrid(board.GetGrids(), selectedUnit, targetGrid);
            }
        }

        //MOVING ONTO AN ENEMY
        else {
            return MoveUnitToGrid(board.GetGrids(), selectedUnit, targetGrid);
        }
    }

    private bool MoveUnitToGrid(Grid[,] gridArray, Unit selectedUnit, Grid targetGrid) {
        /* In the scenario where a piece is on top of us,
           we need to be able to throw them off. */
        selectedUnit = selectedUnit.GetGrid().GetActiveUnit();

        /* CHECK CAN MOVE */
        if (!GetValidGrids(targetGrid).valid) {
            if (debug) { Debug.Log("Move is invalid!"); }
            DeselectOccupants(selectedUnit.GetGrid());
            board = new BoardState(gridArray, board.GetTurnOf(), null);
            return false;
        }

        /* CHECK UNIT TYPE ALLOWANCE */
        if (selectedUnit.GetUnitType().Equals(Unit.UNIT_TYPES.CUBE) && movedCubes.Count >= cubeTurns) {
            if(debug) { Debug.Log("You can't move any more cubes!"); }
            DeselectOccupants(selectedUnit.GetGrid());
            board = new BoardState(gridArray, board.GetTurnOf(), null);
            targetGrid.DeselectColour();
            return false;
        }
        if (selectedUnit.GetUnitType().Equals(Unit.UNIT_TYPES.PYRAMID) && movedPyramids.Count >= pyramidTurns) {
            if (debug) { Debug.Log("You can't move any more pyramids!"); }
            DeselectOccupants(selectedUnit.GetGrid());
            board = new BoardState(gridArray, board.GetTurnOf(), null);
            targetGrid.DeselectColour();
            return false;
        }        

        gridArray = MoveOffGrid(gridArray, selectedUnit, targetGrid);
        Unit[] moreOccupants;

        /* ~~~~~~~ STACK / MOVE TO EMPTY ~~~~~~~~ */
        if (targetGrid.GetActiveUnit() == null || targetGrid.GetActiveUnit().GetTeam().Equals(selectedUnit.GetTeam())) {
            
            moreOccupants = new Unit[targetGrid.GetOccupants().Length + 1];
            for (int i = 0; i < targetGrid.GetOccupants().Length; i++) {
                moreOccupants[i] = targetGrid.GetOccupants()[i];
            }
            moreOccupants[moreOccupants.Length - 1] = selectedUnit;

            selectedUnit.transform.position = new Vector3(targetGrid.GetCoords().x, targetGrid.GetOccupants().Length + (0.5f * selectedUnit.transform.localScale.y), targetGrid.GetCoords().y);
        }

        /* ~~~~~~~ TAKE (REPLACE) ~~~~~~~~ */
        else {
            moreOccupants = new Unit[targetGrid.GetOccupants().Length];
            for (int i = 0; i < targetGrid.GetOccupants().Length; i++) {
                moreOccupants[i] = targetGrid.GetOccupants()[i];
            }
            Unit takenUnit = moreOccupants[moreOccupants.Length - 1];
            moreOccupants[moreOccupants.Length - 1] = selectedUnit;
            if(debug) { Debug.Log("Player " + selectedUnit.GetTeam().ToString() + " has taken a piece"); }
            Destroy(takenUnit.gameObject);

            selectedUnit.transform.position = new Vector3(targetGrid.GetCoords().x, targetGrid.GetOccupants().Length + (0.5f * selectedUnit.transform.localScale.y) - 1, targetGrid.GetCoords().y);
        }

        /* ~~~~~~~ FINISH UP AND CHECK TURN ALLOWANCES ~~~~~~~ */
        targetGrid.SetOccupants(moreOccupants);
        selectedUnit.SetGrid(targetGrid);

        if (selectedUnit.GetUnitType().Equals(Unit.UNIT_TYPES.CUBE)) {
            movedCubes.Add((Cube)selectedUnit);
        } else {
            movedPyramids.Add((Pyramid)selectedUnit);
        }

        ChangeTurn(targetGrid, moreOccupants, selectedUnit);
        return true;
    }

    public void SelectOccupants(Grid occupiedGrid) {
        foreach (Unit unit in occupiedGrid.GetOccupants()) {
            Player.TEAM team = unit.GetTeam();
            unit.GetComponent<Renderer>().material.color = Player.TeamColour(team, true);
        }
    }
    public void DeselectOccupants(Grid occupiedGrid) {
        foreach (Unit unit in occupiedGrid.GetOccupants()) {
            Player.TEAM team = unit.GetTeam();
            unit.GetComponent<Renderer>().material.color = Player.TeamColour(team, false);
        }
    }
    
    private void ChangeTurn(Grid targetGrid, Unit[] moreOccupants, Unit selectedUnit) {
        bool noMoreMoves = (movedCubes.Count >= cubeTurns && movedPyramids.Count >= pyramidTurns);
        if (noMoreMoves) {
            PassTurn();
        } else {
            // allow passing
            canPass = true;
            board = new BoardState(board.GetGrids(), board.GetTurnOf(), null);

            foreach (Player player in players) {
                if (player.GetTeam().Equals(board.GetTurnOf())
                    && player.GetPlayerType().Equals(Player.PLAYER_TYPES.AI)) {
                    ((AIPlayer)player).MakeMove(board);
                }
            }
        }
    }

    public void PassTurn() {
        /* RESET MOVES */
        movedCubes = new List<Cube>();
        movedPyramids = new List<Pyramid>();
        // deny passing
        canPass = false;

        /* CHANGE TURN */
        if (board.GetTurnOf() == Player.TEAM.ZERO) {
            board = new BoardState(board.GetGrids(), Player.TEAM.ONE, null);
        } else {
            board = new BoardState(board.GetGrids(), Player.TEAM.ZERO, null);
        }

        foreach(Player player in players) {
            if(player.GetTeam().Equals(board.GetTurnOf()) 
                && player.GetPlayerType().Equals(Player.PLAYER_TYPES.AI)) {
                ((AIPlayer)player).MakeMove(board);
            }
        }
    }

    private Grid[,] MoveOffGrid(Grid[,] gridArray, Unit selectedUnit, Grid targetGrid) {
        DeselectOccupants(selectedUnit.GetGrid());

        /* REMOVE UNIT FROM GRID */
        Unit[] fewerOccupants = new Unit[selectedUnit.GetGrid().GetOccupants().Length - 1];
        for (int i = 0; i < selectedUnit.GetGrid().GetOccupants().Length - 1; i++) {
            fewerOccupants[i] = selectedUnit.GetGrid().GetOccupants()[i];
        }
        selectedUnit.GetGrid().SetOccupants(fewerOccupants);
        return gridArray;
    }

    public ValidGrids GetValidGrids(Grid targetGrid) {
        if (board.GetSelectedUnit() == null) {
            return new ValidGrids(false, null);
        }
        else {
            return GetValidGrids(board.GetSelectedUnit(), targetGrid);
        }
    }

    public ValidGrids GetValidGrids(Unit trialUnit, Grid targetGrid) {
        List<Grid> validGrids = new List<Grid>();

        Grid originGrid = trialUnit.GetGrid();
        Vector2 move = targetGrid.GetCoords() - originGrid.GetCoords();
        int distance = Mathf.RoundToInt(Mathf.Sqrt(Vector2.SqrMagnitude(move)));
        Vector2 unitVector = move * (1.0f / distance);



        if (board.GetSelectedUnit() == null || (board.GetSelectedUnit().GetComponent<Cube>() != null && distance > 1)) {
            return new ValidGrids(false, null);
        }

        //For moves parallel to the edges
        if ((move.x == 0 || move.y == 0) && distance <= originGrid.GetOccupants().Length) {
            validGrids.Add(targetGrid);
            for (int i = 1; i < distance; i++) {
                Vector2 nextGrid = originGrid.GetCoords() + (i * unitVector);
                validGrids.Add(board.GetGrids()[(int)nextGrid.x, (int)nextGrid.y]);
            }
            return new ValidGrids(true, validGrids);
        }
        //For moves diagonal
        else if ((Mathf.Abs(move.x) == Mathf.Abs(move.y)) && Mathf.Abs(move.x) <= originGrid.GetOccupants().Length) {
            validGrids.Add(targetGrid);
            return new ValidGrids(true, validGrids);
        } else {
            return new ValidGrids(false, null);
        }
    }
    
    public Unit GetSelectedUnit() {
        return board.GetSelectedUnit();
    }
}

public class ValidGrids {
    public bool valid;
    public List<Grid> validGrids;

    public ValidGrids(bool valid, List<Grid> validGrids) {
        this.valid = valid;
        this.validGrids = validGrids;
    }
}



