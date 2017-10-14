using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit {

    protected BoardState.PLAYER team;
    public BoardState.PLAYER GetTeam() {
        return team;
    }    

}
