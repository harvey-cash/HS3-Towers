using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour {

    private GameController controller;
    public void SetController(GameController controller) {
        this.controller = controller;
    }

    protected Grid occupiedGrid;
    public void SetGrid(Grid occupiedGrid) {
        this.occupiedGrid = occupiedGrid;
    }
    public Grid GetGrid() { return occupiedGrid; }

    public enum UNIT_TYPES { CUBE, PYRAMID };
    public abstract UNIT_TYPES GetUnitType();

    protected Player.TEAM team;
    public void SetTeam(Player.TEAM team) {
        this.team = team;
    }
    public Player.TEAM GetTeam() {
        return team;
    }

    private void OnMouseEnter() {
        controller.SelectOccupants(occupiedGrid);
        
    }
    private void OnMouseExit() {
        bool selected = false;
        foreach (Unit unit in occupiedGrid.GetOccupants()) {
            if (controller.GetSelectedUnit() != null && controller.GetSelectedUnit().Equals(unit)) {
                selected = true;
            }
        }

        if(!selected) {
            controller.DeselectOccupants(GetGrid());
        }
    }
    public void DeselectColour() {
        GetComponent<Renderer>().material.color = Player.TeamColour(team, false);
    }

    private void OnMouseUp() {
        controller.OnUnitClick(this);
    }

}
