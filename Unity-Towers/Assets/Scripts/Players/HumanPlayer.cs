﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player {

    public HumanPlayer(TEAM team) {
        this.team = team;
        playerType = PLAYER_TYPES.HUMAN;
    }
    public HumanPlayer(int team) {
        switch (team) {
            case 0: this.team = TEAM.GREEN; break;
            case 1: this.team = TEAM.RED; break;

            default: this.team = TEAM.GREEN; break;
        }
        playerType = PLAYER_TYPES.HUMAN;
    }

    public override void RandomDelay(List<PotentialMove> moves) {
        //No need to wait for humans...
    }
}
