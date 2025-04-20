import express from "express";
import cors from "cors";

const app = express();
const PORT = 3001;

app.use(
  cors({
    origin: "*",
    methods: ["GET", "POST", "OPTIONS"],
    allowedHeaders: ["Content-Type", "ngrok-skip-browser-warning"],
  })
);
app.use(express.json());

let playerActions = {}; // 각 플레이어의 행동을 저장
let lastTurnActions = null; // 지난 턴의 행동을 저장
let lastTurnState = null; // 다음 턴 시작 시 전달할 상태
let playersAcknowledged = new Set(); // 턴 결과를 확인한 플레이어 추적

app.get("/", (req, res) => {
  res.send("Server is running!");
});

// === 전역 상태 ===
const boardSize = 5;
let turnCounter = 0;
const playerOrder = ["player1", "player2"];
let obstacleState = null; // ← 여기서 관리
let currentBlocks = null;
let submittedPlayers = new Set();
let submitCount = { player1: 0, player2: 0 };

const unitState = [
  { id: "A1", angle: 0, x: 0, y: 0, hp: 5, fp: 3, playerId: "player1" },
  { id: "B1", angle: 0, x: 4, y: 0, hp: 5, fp: 3, playerId: "player1" },
  { id: "A2", angle: 180, x: 0, y: 4, hp: 5, fp: 3, playerId: "player2" },
  { id: "B2", angle: 180, x: 4, y: 4, hp: 5, fp: 3, playerId: "player2" },
];

// === 장애물 최초 1회 생성
function initObstacles() {
  if (obstacleState) return; // 이미 생성됨

  const count = Math.floor(Math.random() * 3) + 2; // 2~4개
  obstacleState = [];

  for (let i = 0; i < count; i++) {
    obstacleState.push({
      type: "heal",
      x: Math.floor(Math.random() * boardSize),
      y: Math.floor(Math.random() * boardSize),
    });
  }
}

// === 유틸 함수: 블럭 5장 뽑기
function getOrCreateBlocksForTurn() {
  if (currentBlocks) return currentBlocks;

  currentBlocks = [];
  for (let i = 0; i < 5; i++) {
    const id = Math.floor(Math.random() * 4) + 1;
    currentBlocks.push(id);
  }
  return currentBlocks;
}

function endTurn() {
  turnCounter++;
  currentBlocks = null; // 다음 턴에서 다시 생성됨
  submittedPlayers.clear();
  playerActions = {}; // 다음 턴을 위해 초기화
  lastTurnActions = null;
  playersAcknowledged.clear(); // 확인한 사용자 초기화
  submitCount = { player1: 0, player2: 0 };
  lastTurnState = null; // 턴 상태 초기화
}

// === GET /game-state
app.get("/game-state", (req, res) => {
  const playerId = req.query.playerId;
  if (!playerId || !playerOrder.includes(playerId)) {
    return res.status(400).json({ error: "Invalid or missing playerId" });
  }

  // 🔥 최초 1회만 장애물 생성
  initObstacles();

  res.json({
    data: lastTurnState ?? {
      blocks: getOrCreateBlocksForTurn(),
      obstacles: obstacleState, // 변하지 않음
      units: unitState,
    },
  });
});

// === POST /submit-turn
app.post("/submit-turn", (req, res) => {
  const { playerId } = req.body;
  const { actions } = req.body;
  if (!playerId || !playerOrder.includes(playerId)) {
    return res.status(400).json({ error: "Invalid or missing playerId" });
  }
  if (!actions || !Array.isArray(actions)) {
    return res.status(400).json({ error: "Missing or invalid actions array" });
  }
  playerActions[playerId] = actions;
  submitCount[playerId] += 1;

  submittedPlayers.add(playerId);

  if (submittedPlayers.size === playerOrder.length && !lastTurnActions) {
    lastTurnActions = { ...playerActions }; // ✅ 최초 1회만 저장
  }

  res.json({ message: "Turn submitted. Waiting for the other player." });
});

app.get("/all-player-actions", (req, res) => {
  const playerId = req.query.playerId;
  if (!playerId || !playerOrder.includes(playerId)) {
    return res.status(400).json({ error: "Invalid or missing playerId" });
  }

  if (
    submitCount["player1"] > 0 &&
    submitCount["player2"] > 0 &&
    lastTurnActions
  ) {
    playersAcknowledged.add(playerId);

    const flatActions = [];

    for (const playerId of Object.keys(lastTurnActions)) {
      for (const action of lastTurnActions[playerId]) {
        flatActions.push(action);
      }
    }

    return res.json({ data: { actions: flatActions, playerId: "player1" } });
  }

  return res.json({
    status: "pending",
    message: "Waiting for all players to submit actions.",
  });
});

app.post("/turn-result", (req, res) => {
  // 모든 플레이어가 actions를 본 이후에만 허용
  if (playersAcknowledged.size < playerOrder.length) {
    return res
      .status(400)
      .json({ error: "Not all players have acknowledged the result yet" });
  }

  // lastTurnState = req.body.state; // POST된 상태 저장
  endTurn();
  res.json({ message: "Turn officially ended. Proceed to next turn." });
});

app.listen(PORT, () => {
  console.log(`✅ Server running at http://localhost:${PORT}`);
});
