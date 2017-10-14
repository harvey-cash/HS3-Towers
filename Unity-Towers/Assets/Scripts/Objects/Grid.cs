using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

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
    public Vector2 GetID() {
        return coords;
    }


    private void OnMouseEnter() {
        GetComponent<Renderer>().material.color = Color.blue;
    }
    private void OnMouseExit() {
        GetComponent<Renderer>().material.color = Color.white;
    }
    private void OnMouseUp() {
        Debug.Log("Mouse selected: " + coords.ToString());
    }
}
