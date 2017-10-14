using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    private GameController controller;
    public void SetController(GameController controller) {
        this.controller = controller;
    }

    private Vector2 coords;
    public Vector2 GetCoords() { return coords; }

    private Unit[] occupants;
    public void SetOccupants(Unit[] occupants) {
        this.occupants = occupants;
    }
    public Unit[] GetOccupants() {
        return occupants;
    }

    public void InitGrid(Vector2 coords) {
        this.coords = coords;
        this.occupants = new Unit[0];
    }
    public void InitGrid(Vector2 coords, Unit[] occupants) {
        this.coords = coords;
        this.occupants = occupants;
    }


    private void OnMouseEnter() {
        if (controller.ValidMove(this)) {
            GetComponent<Renderer>().material.color = Color.cyan;
        } else {
            GetComponent<Renderer>().material.color = Color.grey;
        }
        
    }
    private void OnMouseExit() {
        GetComponent<Renderer>().material.color = Color.white;
    }
    private void OnMouseUp() {
        controller.OnGridClick(this);
    }

    public Unit GetActiveUnit() {
        if (occupants.Length > 0) { return occupants[occupants.Length - 1]; } else { return null; }
    }
}

