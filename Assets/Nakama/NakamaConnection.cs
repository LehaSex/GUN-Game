using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class NakamaConnection : ScriptableObject
{
    public string Scheme = "http";
    public string Host = "localhost";
    public int Port = 7350;
    public string ServerKey = "defaultkey";

    private const string SessionPrefName = "nakama.session";
    private const string DeviceIdentifierPrefName = "nakama.deviceUniqueIdentifier";

    public IClient Client;
    public ISession Session;
    public ISocket Socket;
    public IApiAccount Account { get; set; }

    private string currentMatchmakingTicket;
    private string currentMatchId;

    /// <summary>
    /// Подключается к серверу Nakama с использованием аутентификации устройства и открывает сокет для связи в реальном времени.
    /// </summary>
    public async Task Connect()
    {
        FBPP.Start(new FBPPConfig
		{
				SaveFileName = "PlayerPrefs.json",
				AutoSaveData = true,
				ScrambleSaveData = false
		});
        // Подключение к серверу
        Client = new Nakama.Client(Scheme, Host, Port, ServerKey, UnityWebRequestAdapter.Instance);

        // Попытка восстановить существующий сеанс пользователя.
        var authToken = FBPP.GetString(SessionPrefName);
        if (!string.IsNullOrEmpty(authToken))
        {
            var session = Nakama.Session.Restore(authToken);
            if (!session.IsExpired)
            {
                Session = session;
            }
        }

        // Если не смогли восстановить текущий сеанс, создаём новый
        if (Session == null)
        {
            string deviceId;

            // Если мы уже сохранили идентификатор устройства в PlayerPrefs, то используем его
            if (FBPP.HasKey(DeviceIdentifierPrefName))
            {
                deviceId = FBPP.GetString(DeviceIdentifierPrefName);
            }
            else
            {
                // Генерация уникального идентификатора
                deviceId = SystemInfo.deviceUniqueIdentifier;
                if (deviceId == SystemInfo.unsupportedIdentifier)
                {
                    deviceId = System.Guid.NewGuid().ToString();
                }

                // Сохранить идентификатор 
                FBPP.SetString(DeviceIdentifierPrefName, deviceId);
            }

            // Проверка подлинности устройства
            Session = await Client.AuthenticateDeviceAsync(deviceId);

            // Сохранить возвращаемый токен авторизации, чтобы мы могли восстановить сеанс позже, если это необходимо.
            FBPP.SetString(SessionPrefName, Session.AuthToken);
        }

        // Откройте новый сокет для связи в реальном времени.
        Socket = Client.NewSocket();
        await Socket.ConnectAsync(Session, true);
        Account = await Client.GetAccountAsync(Session);
    }

    /// <summary>
    /// Поиск нового матча с минимальным количеством игроков
    /// </summary>
    public async Task FindMatch(int minPlayers = 2)
    {
        // Разобраться.
        var matchmakingProperties = new Dictionary<string, string>
        {
            { "engine", "unity" }
        };

        // Добавляем нового клиента в пулл и ждём
        var matchmakerTicket = await Socket.AddMatchmakerAsync("+properties.engine:unity", minPlayers, minPlayers, matchmakingProperties);
        currentMatchmakingTicket = matchmakerTicket.Ticket;
    }

    // Я сейчас умру
    /// <summary>
    /// Отменяет текущий запрос на подбор партнеров.
    /// </summary>
    public async Task CancelMatchmaking()
    {
        await Socket.RemoveMatchmakerAsync(currentMatchmakingTicket);
    }
}
