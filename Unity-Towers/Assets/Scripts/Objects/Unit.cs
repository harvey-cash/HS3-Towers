using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour {

    public enum UNIT_TYPES { CUBE, PYRAMID };

    protected Player.TEAM team;
    public void SetTeam(Player.TEAM team) {
        this.team = team;
    }
    public Player.TEAM GetTeam() {
        return team;
    }

    private void OnMouseEnter() {
        GetComponent<Renderer>().material.color = Player.TeamColour(team);
    }
    private void OnMouseExit() {
        GetComponent<Renderer>().material.color = Color.white;
    }

}
