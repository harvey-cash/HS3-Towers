using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player {

    protected TEAM team;
    public TEAM GetTeam() { return team; }
	public enum TEAM { ZERO, ONE };

    protected PLAYER_TYPES playerType;
    public PLAYER_TYPES GetPlayerType() { return playerType; }
    public enum PLAYER_TYPES { HUMAN, AI };

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
            case PLAYER_TYPES.AI: return new AIPlayer(team);

            default: return new HumanPlayer(team);
        }
    }

    public abstract void RandomDelay(List<PotentialMove> moves);

}
