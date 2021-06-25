using UnityEngine;

[CreateAssetMenu(fileName = "Assets", menuName = "Foodfight/Game Config", order = 2)]
public class GameConfig : ScriptableObject {

    public int numberOfPlayers = 2;

    public string player1Name = "Player 1";
    public string player2Name = "Player 2";

    public int player1Score = 0;
    public int player2Score = 0;

    public GameState gameState = GameState.Menu;

    public ParticleSystem collisionParticle;

    public void Reset() {
        player1Score = 0;
        player2Score = 0;
        gameState = GameState.Play;
    }
}


public enum GameState
{
    Init, Menu, Play, Pause, GameOver
}

[System.Serializable]
public enum Player
{
    One, Two
}