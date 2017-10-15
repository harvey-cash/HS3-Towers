using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player {

    public AIPlayer(TEAM team) {
        this.team = team;
        playerType = PLAYER_TYPES.AI;
    }
    public AIPlayer(int team) {
        switch (team) {
            case 0: this.team = TEAM.GREEN; break;
            case 1: this.team = TEAM.RED; break;

            default: this.team = TEAM.GREEN; break;
        }
        playerType = PLAYER_TYPES.AI;
    }

    private const int MAX_ATTEMPTS = 20;
    private float minWait = 1.0f, maxWait = 2.0f;

    private Vector2[] directions = {
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0),
        new Vector2(1, -1),
        new Vector2(0, -1),
        new Vector2(-1, -1),
        new Vector2(-1, 0),
        new Vector2(-1, 1)
    };

    public void MakeMove(BoardState board) {
        Debug.Log("AI IS MAKING A MOVE");

        // first, let's get a list of all of our active units
        List<Unit> activeUnits = new List<Unit>();
        foreach(Grid grid in board.GetGrids()) {
            if(grid.GetActiveUnit(team) != null) {
                activeUnits.Add(grid.GetActiveUnit());
            }
        }

        // what are our potential moves with them?
        List<PotentialMove> potentialMoves = new List<PotentialMove>();
        foreach (Unit unit in activeUnits) {
            foreach (Vector2 direction in directions) {
                int distance = unit.GetGrid().GetOccupants().Length;

                for(int i = 1; i <= distance; i++) {
                    Vector2 coords = unit.GetGrid().GetCoords() + (i * direction);

                    bool outOfX = coords.x < 0 || coords.x >= board.GetGrids().GetLength(0);
                    bool outOfY = coords.y < 0 || coords.y >= board.GetGrids().GetLength(1);
                    if (outOfX || outOfY) { break; }

                    Grid targetGrid = board.GetGrids()[(int)coords.x, (int)coords.y];
                    PotentialMove move = new PotentialMove(unit, targetGrid);
                    potentialMoves.Add(move);
                    ValidGrids grids = controller.GetValidGrids(unit, targetGrid);

                    if (grids.valid) {
                        potentialMoves.Add(new PotentialMove(unit, targetGrid));
                    }
                }                
            }
        }

        // if any of those are an enemy, take it
        List<PotentialMove> enemies = new List<PotentialMove>();
        foreach(PotentialMove move in potentialMoves) {
            if(move.targetGrid.GetActiveUnit() != null 
                && !move.targetGrid.GetActiveUnit().GetTeam().Equals(team)) {
                enemies.Add(move);
            }
        }
        if (enemies.Count > 0) {
            controller.RandomDelay(this, minWait, maxWait, enemies);
        } 
        
        // okay, no enemies in range, move toward one then
        else {
            controller.RandomDelay(this, minWait, maxWait, potentialMoves);
        }
    }

    public override void RandomDelay(List<PotentialMove> potentialMoves) {
        for (int i = 0; i < MAX_ATTEMPTS; i++) {
            int index = Random.Range(0, potentialMoves.Count - 1);
            if (controller.OnUnitClick(potentialMoves[index].unit)
                && controller.OnGridClick(potentialMoves[index].targetGrid)) {
                return;
            }
        }

        //if it gets here, it was unable to find a valid move. Try and pass?
        if (controller.GetCanPass()) {
            controller.PassTurn();
            Debug.Log("AI HAS PASSED");
        } else {
            Debug.Log("AI GIVES UP. PLZ MOVE FOR ME");
        }
    }

}

public class PotentialMove {
    public Unit unit;
    public Grid targetGrid;

    public PotentialMove(Unit unit, Grid targetGrid) {
        this.unit = unit;
        this.targetGrid = targetGrid;
    }

    public override string ToString() {
        return "Unit at: " + unit.GetGrid().GetCoords() + 
            " to: " + targetGrid.GetCoords().ToString();        
    }
}

