
public class GameResult 
{
    public bool GameOver { get; private set; }

    public Player WPlayer { get; private set; }


    public GameResult(bool gameOver, Player wPlayer)
    {
        GameOver = gameOver;
        WPlayer = wPlayer;
        
    }
}
