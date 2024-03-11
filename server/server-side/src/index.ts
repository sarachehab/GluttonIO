// import { connectToDB } from '../db.js'; // Assuming you have a separate file for DB connection
import { config } from "dotenv";
import { WebSocketServer, WebSocket, RawData } from "ws";
import { GameState } from "../classes/Game.js";
import { ClientMsgType, ServerMsgType } from "../classes/MessageType.js";

import * as uuid from "uuid";

import { PlayerUtils } from "../utils/PlayerUtils.js";

const handleWsMessage = (
  game: GameState,
  socket: WebSocket,
  socketId: string,
  msg: RawData
) => {
  try {
    const msgJson = JSON.parse(msg.toString("utf8"));

    if (msgJson.type == ClientMsgType.Join) {
      game.AddPlayer(socket, socketId, msgJson.data);
      return;
    }

    if (!game.players[socketId]) {
      return;
    }

    switch (msgJson.type) {
      case ClientMsgType.UpdatePosition:
        // game.UpdatePlayerPosition(socketId, msgJson.data);
        PlayerUtils.HandleUpdatePlayerPosition(game, socketId, msgJson.data);
        break;

      case ClientMsgType.PlayerEatenFood:
        PlayerUtils.HandlePlayerEatenFood(game, socketId, msgJson.data);
        break;

      case ClientMsgType.PlayerEatenEnemy:
        console.log("Player ate enemy");
        PlayerUtils.HandlePlayerEatenEnemy(game, socketId, msgJson.data);
        break;

      default:
        console.log("Unknown message type:", msgJson.type);
        break;
    }
  } catch (error) {
    console.error("Error parsing JSON:", error);
  }
};

const simulate = (game: GameState, interval: number) => {
  // Add bots every interval
  game.AddBot();
  // setInterval(() => {
  // }, interval);

  game.Init();
};

// Initialize DB connection
const main = async () => {
  const PORT = 8080;
  config();

  const ws = new WebSocketServer({ port: PORT });
  const game = new GameState(1, ws);

  simulate(game, 5000);

  ws.on("listening", () => {
    console.log(`listening to ws connections on port ${PORT}`);
  });

  ws.on("connection", async (socket) => {
    const socketId = uuid.v4();
    game.InitPlayerJoined(socket, socketId);

    // setTimeout(() => {
    //   socket.close();
    // }, 2000);

    socket.on("message", (msg) => handleWsMessage(game, socket, socketId, msg));

    socket.on("close", () => {
      game.RemovePlayer(socketId);
    });

    socket.on("error", (err) => {
      game.RemovePlayer(socketId);
    });
  });
};

main();
