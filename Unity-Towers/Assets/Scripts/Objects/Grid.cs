﻿using System.Collections;
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
        if (controller.GetValidGrids(this).valid) {
            /*
            foreach (Grid grid in controller.GetValidGrids(this).validGrids) {
                grid.GetComponent<Renderer>().material.color = Color.cyan;
            }
            */
            GetComponent<Renderer>().material.color = Color.cyan;
        } else {
            GetComponent<Renderer>().material.color = Color.grey;
        }        
    }
    private void OnMouseExit() {
        DeselectColour();
    }
    public void DeselectColour() {
        GetComponent<Renderer>().material.color = Color.white;
    }

    private void OnMouseUp() {
        controller.OnGridClick(this);
    }

    public Unit GetActiveUnit() {
        if (occupants.Length > 0) { return occupants[occupants.Length - 1]; } else { return null; }
    }
    public Unit GetActiveUnit(Player.TEAM team) {
        Unit activeUnit = null;
        for(int i = 0; i < occupants.Length; i++) {
            if(occupants[i].GetTeam().Equals(team)) {
                activeUnit = occupants[i];
            }
        }
        return activeUnit;
    }
}

