using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController {

    private BoardState boardState;
	public ObjectController(BoardState board) {
        this.boardState = board;
    }

    /* Create the physical board */
    private List<GameObject> grids = new List<GameObject>();
    public void DrawGrids() {
        foreach(GameObject thing in grids) {
            Object.Destroy(thing);
        }
        grids = new List<GameObject>();

        for(int x = 0; x < boardState.GetBoard().GetLength(0); x++) {
            for (int y = 0; y < boardState.GetBoard().GetLength(1); y++) {
                Grid grid = boardState.GetBoard()[x, y];
                GameObject thing = (GameObject)Object.Instantiate(Resources.Load("Grid"));                

                thing.transform.position = new Vector3(grid.GetCoords().x, -1, grid.GetCoords().y);
                grids.Add(thing);
            }
        }
    }
}
