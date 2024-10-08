/// <summary>
/// Must agree with the server's schema.
/// </summary>

public enum ClientMsgType
{
    Join,
    UpdatePosition,
    PlayerEatenFood,
    PlayerEatenEnemy,
    PlayerThrewMass
    
}

public enum ServerMsgType {
    InitSocketId, // 0
    PlayerJoined, // 1
    PlayerLeft, // 2
    UpdatePlayersPosition, // 3
    FoodAdded, // 4
    PlayerAteFood, // 5
    PlayerAteEnemy, // 6,
    PlayerThrewMass // 7

}

// Join message data
public class JoinMsgData
{
    public string playerId;

    public JoinMsgData(string playerId)
    {
        this.playerId = playerId;
    }
}