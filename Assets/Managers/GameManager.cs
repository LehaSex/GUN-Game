using Nakama;
using Nakama.TinyJson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс обрабатывающий логику игры
/// </summary>
public class GameManager : MonoBehaviour
{
    public NakamaConnection NakamaConnection;
    public GameObject NetworkLocalPlayerPrefab;
    public GameObject NetworkRemotePlayerPrefab;
    public GameObject LootPrefab;
    public GameObject MainMenu;
    public GameObject InGameMenu;
    public Text WinningPlayerText;
    public Text Coins;
    public GameObject CoinIcon;
    public GameObject SpawnPoints;
    public GameObject randomSpawnManager;
    public AudioManager AudioManager;
    public string allWeaponData;

    private IDictionary<string, GameObject> players;
    private IUserPresence localUser;
    private GameObject localPlayer;
    private IMatch currentMatch;

    private Transform[] spawnPoints;
    private string localDisplayName;

    /// <summary>
    /// Вызывается при запуске
    /// </summary>
    private async void Start()
    {
        // Создайть пустой словарь для хранения ссылок на подключенных в данный момент игроков.
        players = new Dictionary<string, GameObject>();
        // Получить ссылку на UnityMainThreadDispatcher.
        // Это нужно для поставки в очередь событий в основном потоке.
        // Если это не сделать то мы не сможем создавать новые экземпляры объектов и изменять напр. UI
        var mainThread = UnityMainThreadDispatcher.Instance();

        // Подключение к накаме
        await NakamaConnection.Connect();

        // Включить кнопку войти в игру
        MainMenu.GetComponent<MainMenu>().EnableFindMatchButton();
        CoinIcon.SetActive(true);
        Coins.text = GetWallet(NakamaConnection.Account.Wallet).ToString();

        IApiStorageObjects result = await NakamaConnection.Client.ReadStorageObjectsAsync(
                NakamaConnection.Session,
                ReadSystemData("system", "weapons"));
        allWeaponData = result.Objects.ElementAt(0).Value;
        Debug.Log("All Weapon Data Loaded: " + allWeaponData);
        PlayerPrefs.SetString("WeaponData", allWeaponData);

        // Обработчики сетевых событий
        NakamaConnection.Socket.ReceivedMatchmakerMatched += m => mainThread.Enqueue(() => OnReceivedMatchmakerMatched(m));
        NakamaConnection.Socket.ReceivedMatchPresence += m => mainThread.Enqueue(() => OnReceivedMatchPresence(m));
        NakamaConnection.Socket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));

        // Обработчик событий внутриигрового меню
        InGameMenu.GetComponent<InGameMenu>().OnRequestQuitMatch.AddListener(async () => await QuitMatch());
    }

    private StorageObjectId[] ReadSystemData(string collection, string key)
    {
        StorageObjectId[] objs = new StorageObjectId[]
        {
            new StorageObjectId
            {
                Collection = collection,
                Key = key,
                UserId = ""
            }
        };
        return objs;
    }

    private int GetWallet(string wallet)
		{
			Dictionary<string, int> currency = wallet.FromJson<Dictionary<string, int>>();

			if (currency.ContainsKey("coins"))
			{
				return currency["coins"];
			}

			return 0;
		}

    /// <summary>
    /// Вызывается когда с сервера получено сообщение Matchmaker Matched.
    /// </summary>
    /// <param name="matched">The MatchmakerMatched data.</param>
    private async void OnReceivedMatchmakerMatched(IMatchmakerMatched matched)
    {
        // Кэшировать ссылку на локального пользователя
        localUser = matched.Self.Presence;

        // Подключение к матчу
        var match = await NakamaConnection.Socket.JoinMatchAsync(matched);

        // Отключить главное меню и включить игровой UI (изменить на пересылку в другую сцену)
        // Переделать!!!
        MainMenu.GetComponent<MainMenu>().DeactivateMenu();
        InGameMenu.SetActive(true);

        // Играть музыку.
        AudioManager.PlayMatchTheme();
        randomSpawnManager.GetComponent<RandomSpawnManager>().LootSpawn.AddListener(SpawnLoot);
        // Заспавнить каждого подключенного юзера
        foreach (var user in match.Presences)
        {
            SpawnPlayer(match.Id, user);
        }
        randomSpawnManager.GetComponent<RandomSpawnManager>().StartSpawner();
        // Кэшировать ссылку на текущий матч
        currentMatch = match;
    }

    /// <summary>
    /// Вызывается, когда игрок/игроки присоединяются к матчу или покидают его.
    /// </summary>
    /// <param name="matchPresenceEvent">The MatchPresenceEvent data.</param>
    private void OnReceivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        // Для каждого нового пользователя, который присоединяется, создавать для него персонажа.
        foreach (var user in matchPresenceEvent.Joins)
        {
            SpawnPlayer(matchPresenceEvent.MatchId, user);
        }

        // Для каждого игрока, который уходит, удалить его персонажа.
        foreach (var user in matchPresenceEvent.Leaves)
        {
            if (players.ContainsKey(user.SessionId))
            {
                Destroy(players[user.SessionId]);
                players.Remove(user.SessionId);
            }
        }
    }

    /// <summary>
    /// Вызывается при новом состоянии матча.
    /// </summary>
    /// <param name="matchState">The MatchState data.</param>
    private async Task OnReceivedMatchState(IMatchState matchState)
    {
        // Получить ID сессии для локального юзера.
        var userSessionId = matchState.UserPresence.SessionId;

        // Если объект matchState имеет длину то расшифровать
        var state = matchState.State.Length > 0 ? System.Text.Encoding.UTF8.GetString(matchState.State).FromJson<Dictionary<string, string>>() : null;

        // Действие скрипт Player/OpCodes 
        switch(matchState.OpCode)
        {
            case OpCodes.Died:
                // ПЕРЕДЕЛАТЬ!!!!!!!
                // Получить ссылку на игрока который сдох и через 0.5 секунд удалить его из массива игроков
                var playerToDestroy = players[userSessionId];
                Destroy(playerToDestroy, 0.5f);
                players.Remove(userSessionId);

                // Если игрок остался один то объявить победителя и начать некст раунд
                if (players.Count == 1 && players.First().Key == localUser.SessionId) {
                    AnnounceWinnerAndStartNewRound();
                }
                break;
            case OpCodes.Respawned:
                // Спавнить игрока по индексу спавна
                SpawnPlayer(currentMatch.Id, matchState.UserPresence, int.Parse(state["spawnIndex"]));
                break;
            case OpCodes.NewRound:
                // Отобразить имя победителя и начать новый раунд
                await AnnounceWinnerAndRespawn(state["winningPlayerName"]);
                break;
            case OpCodes.LootSpawned:
                    var crate = Instantiate(LootPrefab, new Vector3(float.Parse(state["loot.x"]), float.Parse(state["loot.y"]), float.Parse(state["loot.z"])), transform.rotation * Quaternion.Euler (0f, 180f, 0f));
                    Destroy (crate, 10.0f);
                break;
            default:
                break;
        }
    }



    /// <summary>
    /// Спавнит игрока
    /// </summary>
    /// <param name="matchId">The match the player is connected to.</param>
    /// <param name="user">The player's network presence data.</param>
    /// <param name="spawnIndex">The spawn location index they should be spawned at.</param>
    private void SpawnPlayer(string matchId, IUserPresence user, int spawnIndex = -1)
    {
        // Если игрок уже заспавнен
        if (players.ContainsKey(user.SessionId))
        {
            return;
        }

        // Если индекс возрождения равен -1, то выберать точку возрождения случайно, иначе создайть игрока в указанной точке возрождения.
        var spawnPoint = spawnIndex == -1 ?
            SpawnPoints.transform.GetChild(Random.Range(0, SpawnPoints.transform.childCount - 1)) :
            SpawnPoints.transform.GetChild(spawnIndex);

        // Проверка игрока локальный он или кто на основе Session ID 
        var isLocal = user.SessionId == localUser.SessionId;

        // Выбрать префаб игрока в зависимости локальный или не
        var playerPrefab = isLocal ? NetworkLocalPlayerPrefab : NetworkRemotePlayerPrefab;

        // Спавнить игрока
        var player = Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity);

        // Установка соотв значений сетевых если это удаленный игрок
        // это ВАЖНО!!!!!
        if (!isLocal)
        {
            player.GetComponent<PlayerNetworkRemoteSync>().NetworkData = new RemotePlayerNetworkData
            {
                MatchId = matchId,
                User = user
            };
        }

        // Добавить игрока в массив игроков
        players.Add(user.SessionId, player);

        // Если это наш локальный перс то слушаем события его смертей
        if (isLocal)
        {
            localPlayer = player;
            player.GetComponent<PlayerHealthController>().PlayerDied.AddListener(OnLocalPlayerDied);
        }

        // Дать цвет игроку в зависимости от его номера в массиве игроков
    //    player.GetComponentInChildren<PlayerColorController>().SetColor(System.Array.IndexOf(players.Keys.ToArray(), user.SessionId));
    }

    /// <summary>
    /// Вызывается когда локальный игрок сдох
    /// </summary>
    /// <param name="player">The local player.</param>
    private async void OnLocalPlayerDied(GameObject player)
    {
        // Отправляем всем сообщение что мы сдохли
        await SendMatchStateAsync(OpCodes.Died, MatchDataJson.Died(player.transform.position));

        // Удалить нас из массива игроков и удалить перса через 0.5 сек
        players.Remove(localUser.SessionId);
        Destroy(player, 0.5f);
    }

    private async void SpawnLoot(Vector3 spawnIndex)
    {
        await SendMatchStateAsync(OpCodes.LootSpawned, MatchDataJson.LootSpawned(spawnIndex));
    }

    /// <summary>
    /// Отправляет сообщение на серв что игрок победил и начинается новый раунд
    /// </summary>
    /// <returns></returns>
    public async void AnnounceWinnerAndStartNewRound()
    {
        // Получить имя победившего игрока, это будет вызвано только в том случае, если мы станем победителями, так что можно взять наше имя отсюда.
        // Можно так сделать
        // var account = await NakamaConnection.Client.GetAccountAsync(NakamaConnection.Session);
        // var winningPlayerName = account.User.DisplayName;
        // Но это хуёво работает при локальной отладке
        var winningPlayerName = localDisplayName;

        // Сообщение всем что мы победили
        await SendMatchStateAsync(OpCodes.NewRound, MatchDataJson.StartNewRound(winningPlayerName));

        // Отобразить сообщение победившего игрока и возродите нашего игрока.
        await AnnounceWinnerAndRespawn(winningPlayerName);
    }

    /// <summary>
    /// Отображает сообщение о победившем игроке и возрождает игрока.
    /// </summary>
    /// <param name="winningPlayerName">Победивший игрок.</param>
    private async Task AnnounceWinnerAndRespawn(string winningPlayerName)
    {
        // Текст с победителем
        WinningPlayerText.text = string.Format("{0} победил в этом раунде!", winningPlayerName);
        HandleAddCoins();
        // Ждать 2 сек
        await Task.Delay(2000);
        Coins.text = GetWallet(NakamaConnection.Account.Wallet).ToString();

        // Ресетнуть текст с победителем
        WinningPlayerText.text = "";

        // Удалить себя из списка игроков и уничтожить нашего персонажа.
        players.Remove(localUser.SessionId);
        Destroy(localPlayer);

        // Выбрать новую позицию для возрождения и создайть нашего локального игрока.
        var spawnIndex = Random.Range(0, SpawnPoints.transform.childCount - 1);
        SpawnPlayer(currentMatch.Id, localUser, spawnIndex);

        // Рассказать всем, где мы возродились.
        SendMatchState(OpCodes.Respawned, MatchDataJson.Respawned(spawnIndex));
    }

    /// <summary>
    /// Покинуть матч
    /// </summary>
    public async Task QuitMatch()
    {
        // Попросите Накаму покинуть матч.
        await NakamaConnection.Socket.LeaveMatchAsync(currentMatch);

        // Сбросить текущие переменные Match и LocalUser.
        currentMatch = null;
        localUser = null;

        // Уничтожьте все существующие GameObjects игрока.
        foreach (var player in players.Values)
        {
            Destroy(player);
        }

        // Очистить массив игроков.
        players.Clear();

        // Показать главное меню, скрыть игровое меню.
        MainMenu.SetActive(true);
        randomSpawnManager.GetComponent<RandomSpawnManager>().StopSpawner();
        //MainMenu.UICam.SetActive(true);
        InGameMenu.SetActive(false);

        // Играть музыку в меню
        AudioManager.PlayMenuTheme();
    }

    

    	/// <summary>
		/// Добавить монетки
		/// </summary>
		public async void HandleAddCoins()
		{
			try
			{
				IApiRpc newCoins = await NakamaConnection.Client.RpcAsync(NakamaConnection.Session, "add_user_gems");
                NakamaConnection.Account = await NakamaConnection.Client.GetAccountAsync(NakamaConnection.Session);
                Debug.Log("User wallet: " + NakamaConnection.Account.Wallet);
			}
			catch (ApiResponseException e)
			{
				Debug.LogError("Error adding user coins: " + e.Message);
			}
		}

    /// <summary>
    /// Отправляет состояние матча по сетм
    /// </summary>
    /// <param name="opCode">Код операции.</param>
    public async Task SendMatchStateAsync(long opCode, string state)
    {
        await NakamaConnection.Socket.SendMatchStateAsync(currentMatch.Id, opCode, state);
    }

    /// <summary>
    /// Отправляет состояние матча по сетм
    /// </summary>
    public void SendMatchState(long opCode, string state)
    {
        NakamaConnection.Socket.SendMatchStateAsync(currentMatch.Id, opCode, state);
    }

    /// <summary>
    /// Задает отображаемое имя локального пользователя.
    /// </summary>
    /// <param name="displayName">Новое имя для локального игрока</param>
    public void SetDisplayName(string displayName)
    {
        // Мы могли бы установить это на нашем клиенте Nakama, используя приведенный ниже код:
        // await NakamaConnection.Client.UpdateAccountAsync(NakamaConnection.Session, null, displayName);
        // Однако, поскольку мы используем аутентификацию по идентификатору устройства, при локальном запуске 2 или более клиентов они оба будут отображать одно и то же имя при тестировании / отладке.
        // Итак, в этом случае мы просто установим вместо этого локальную переменную.
        localDisplayName = displayName;
    }
}
