using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player {

    protected TEAM team;
	public enum TEAM { ZERO, ONE };
    public enum PLAYER_TYPES { HUMAN };

    protected GameController controller;
    public void SetGameController(GameController controller) {
        this.controller = controller;
    }

    public static Color TeamColour(TEAM team, bool highlight) {
        switch(team) {
            case TEAM.ZERO:
                if(highlight) { return Color.red; }
                else { return new Color(1, 0.4f, 0.4f, 1); }
            case TEAM.ONE:
                if (highlight) { return Color.green; } 
                else { return new Color(0.4f, 1, 0.4f, 1); }
                

            default:
                if (highlight) { return Color.black; }
                else { return Color.grey; }
        }
    }

    public static Player NewPlayer(PLAYER_TYPES type, TEAM team) {
        switch (type) {
            case PLAYER_TYPES.HUMAN: return new HumanPlayer(team);

            default: return new HumanPlayer(team);
        }
    }

}
