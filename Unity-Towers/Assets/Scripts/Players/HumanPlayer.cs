using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player {

    public HumanPlayer(TEAM team) {
        this.team = team;
    }
    public HumanPlayer(int team) {
        switch (team) {
            case 0: this.team = TEAM.ZERO; break;
            case 1: this.team = TEAM.ONE; break;

            default: this.team = TEAM.ZERO; break;
        }
    }

}
