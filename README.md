# GUN! Unity Game

## 💬 What is this?
GUN! is a 2-4 player online game built in the Unity game engine, using [Nakama Open Source Game Server](https://heroiclabs.com/nakama-opensource/)

## 📷 Screenshots
### Menu screenshot:
![MainMenu](https://raw.githubusercontent.com/LehaSex/GUN-Game/master/Screen_Menu.png "Main Menu")
### In-Game screenshot:
![InGame](https://raw.githubusercontent.com/LehaSex/GUN-Game/master/Ingame_Screen.png "In Game")

## 🕹️ Controls

- **A** or **Left Arrow** - Move Left
- **D** or **Right Arrow** - Move Right
- **W** or **Up Arrow** - Jump
- **S** or **Down Arrow** - Jump Down
- **Space** or **Left Mouse**- Shot (Hold to auto fire)

## 🛠️ Constants & Configuration
You can configure the connection to the server in NakamaConnection.cs

![Connection](https://raw.githubusercontent.com/LehaSex/GUN-Game/master/Connection_Screen.png "Connection Setup")

The Docker Engine is required. Follow the Heroic Labs <a href="https://heroiclabs.com/docs/install-docker-quickstart/">quickstart guide</a>.

To start the game server and database once Docker is setup navigate to the `.\ServerModules` folder and run:

```
docker-compose up
```

To modify the Typescript remote procedure calls (RPCs), install Node Package Manager (NPM), run `npm install` and `npx tsc` from the `ServerModules` folder, and restart the server.


