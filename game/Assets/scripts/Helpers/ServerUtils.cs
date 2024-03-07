using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[Serializable]
public class ServerUtils
{
    public static void HandlePlayerJoined(PlayersManager pmInst, object msgData) {
        Debug.Log("Player joined!");
        // pmInst.AddPlayer(
        //     (string)msgData.socketId, 
        //     new Position(msgData.position.x, msgData.position.y)
        // );
    }

    public static void HandleUpdatePlayersPosition(PlayersManager pmInst, object msgData)
    {
        var data = JsonConvert.DeserializeObject<Dictionary<string, object>[]>(msgData.ToString());
        foreach (var player in data)
        {
            if ((string)player["socketId"] == pmInst.selfSocketId) continue;
            
            // Debug.Log("Updating player position: " + player["socketId"] + " " + player["position"].ToString());
            Position pos = JsonConvert.DeserializeObject<Position>(player["position"].ToString());
            pmInst.UpdatePlayerPosition(
                (string)player["socketId"],
                pos.x,
                pos.y
            );
        }
    }
}