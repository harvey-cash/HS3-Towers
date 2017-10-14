using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit {

    public enum UNIT_TYPES { CUBE, PYRAMID };

    protected Player.TEAM team;
    public Player.TEAM GetTeam() {
        return team;
    }

    public static Unit NewUnit(UNIT_TYPES type, Player.TEAM team) {
        switch(type) {
            case UNIT_TYPES.CUBE: return new Cube(team);
            case UNIT_TYPES.PYRAMID: return new Pyramid(team);

            default: return new Cube(team);
        }
    }

}
