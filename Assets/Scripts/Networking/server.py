import socket, random
from _thread import *

class Game:
    def __init__(self, code, maxPlayers, host):
        self.code = code
        self.maxPlayers = maxPlayers
        self.host = host    # id of host of game

        self.playerIDs = [] # list of player ids in game
        self.turnPositions = {}

        self.active = False
        self.full = False
        self.turn = 0
        self.moveCounter = 0

        self.lastMove = []

    def SetTurnPositions(self):
        t = []
        for i in range(self.maxPlayers):
            t.append(i)
        random.shuffle(t)
        for i in range(self.maxPlayers):
            self.turnPositions[self.playerIDs[i]] = t[i]

    def AddPlayer(self, ID):
        self.playerIDs.append(ID)
        if len(self.playerIDs) >= self.maxPlayers:
            self.full = True
            print("Game full")
        print(f"Added player: {ID}, now player IDs: {self.playerIDs}")
        return ID

    def Disconnect(self, id):
        self.playerIDs.remove(id)
        self.full = False
        if len(self.playerIDs) > 0 and id == self.host:
            self.host = self.playerIDs[0]
        print(f"Removed player: {id}, now player IDs: {self.playerIDs}")

    def Encode(self):
        data = ""
        data += str(self.turn) 
        data += "~" + str(self.moveCounter) 
        data += "~" + str(len(self.playerIDs)) 
        data += "~" + str(self.maxPlayers)
        return data

    def Decode(self, msg):
        msg = msg.split("~")
        self.lastMove = [(msg[0],msg[1]), (msg[2], msg[3])]
        self.turn = (self.turn + 1) % self.maxPlayers
        self.moveCounter += 1

    def FetchMove(self):
        return f"{self.lastMove[0][0]}~{self.lastMove[0][1]}~{self.lastMove[1][0]}~{self.lastMove[1][1]}"

VERSION = "0.0"

server = ""
port = 5555

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

try:
    s.bind(('',port))
except socket.error as e:
    print(str(e))

s.listen()
print("Waiting for a connection...")

connected = set()
games = {}
players = -1

codeCharacterValues = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'

def threaded_client(conn, playerID):
    gameCode = ""
    while True:
        try:
            msg = conn.recv(1024).decode().split("::")
            response = "null"

            kick = False
            if msg[0] == "version_check":
                if msg[1] == VERSION:
                    response = "true"
                else:
                    if float(VERSION) > float(msg[1]):
                        response = "false_c"
                    else:
                        response = "false_s"
                    kick = True

            elif msg[0] == "create_game":
                while True:
                    code = random.choice(codeCharacterValues) + random.choice(codeCharacterValues) + random.choice(codeCharacterValues) + random.choice(codeCharacterValues)
                    if code not in games:
                        games[code] = Game(code, int(msg[1]), playerID)
                        break
                print(f"new game created with code {code}")
                response = code

            elif msg[0] == "get_game":
                if msg[1] in games:
                    response = games[msg[1]].Encode()

            elif msg[0] == "get_game_info":
                if msg[1] in games:
                    response = games[msg[1]].code + " " + str(len(games[msg[1]].playerIDs)) + " " + str(games[msg[1]].maxPlayers)

            elif msg[0] == "get_game_started":
                if msg[1] in games:
                    if games[msg[1]].active:
                        response = "true"
                if response != "true":
                    response = "false"

            elif msg[0] == "get_game_code":
                if msg[1] in games:
                    response = msg[1]
                    if games[msg[1]].full:
                        response = "full"

            elif msg[0] == "join_game":
                if msg[1] in games:
                    if not games[msg[1]].full and not games[msg[1]].active:
                        response = str(games[msg[1]].AddPlayer(playerID)) 
                        gameCode = games[msg[1]].code
                        print(f"{playerID} {addr} joined game {gameCode}")
                    elif games[msg[1]].full:
                        response = "full"

            elif msg[0] == "send_move":
                if msg[1] in games:
                    games[msg[1]].Decode(msg[2])
                    response = "true"

            elif msg[0] == "fetch_move":
                if msg[1] in games:
                    response = games[msg[1]].FetchMove()

            elif msg[0] == "start_game":
                if msg[1] in games:
                    if not games[msg[1]].active and games[msg[1]].full:
                        games[msg[1]].active = True
                        games[msg[1]].SetTurnPositions()
                        response = "true"
                        print(f"Started game {msg[1]}")
            
            elif msg[0] == "leave_game":
                if msg[1] in games:
                    games[msg[1]].Disconnect(playerID)
                    if len(games[gameCode].playerIDs) <= 0:
                        games.pop(gameCode)
                        response = "true"
                        print(f"Game {gameCode} deleted.")

            elif msg[0] == "get_host":
                if msg[1] in games:
                    response = str(games[msg[1]].host == playerID)

            elif msg[0] == "get_turn_pos":
                if msg[1] in games:
                    response = str(games[msg[1]].turnPositions[playerID])

            conn.send(response.encode())

            if kick:
                conn.close()
        except:
            print(f"{playerID} {addr} disconnected forcefully.")
            # print(e)
            break
    if gameCode in games:
        games[gameCode].Disconnect(playerID)
        if len(games[gameCode].playerIDs) <= 0:
            games.pop(gameCode)
            print(f"Game {gameCode} deleted.")
    conn.close()

while True:
    conn, addr = s.accept()
    if players < 999999:
        players += 1
    else:
        players = 0
    print("Connected to: ", addr)
    start_new_thread(threaded_client, (conn, players))