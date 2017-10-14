using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player {

    protected TEAM team;
	public enum TEAM { ZERO, ONE };
    public enum PLAYER_TYPES { HUMAN };

    public static Player NewPlayer(PLAYER_TYPES type, TEAM team) {
        switch (type) {
            case PLAYER_TYPES.HUMAN: return new HumanPlayer(team);

            default: return new HumanPlayer(team);
        }
    }

}
