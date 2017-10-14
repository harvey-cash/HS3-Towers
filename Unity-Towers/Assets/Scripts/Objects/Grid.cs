using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid {

    private Vector2 coords;
    private Unit[] occupants;

    public Grid(Vector2 coords) {
        this.coords = coords;
        this.occupants = new Unit[0];
    }
    public Grid(Vector2 coords, Unit[] occupants) {
        this.coords = coords;
        this.occupants = occupants;
    }
    public Vector2 GetID() {
        return coords;
    }    

    public bool MoveHere(Unit unit) {
        return false;
    }
}
