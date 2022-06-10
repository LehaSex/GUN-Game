using System.Collections.Generic;
using Nakama.TinyJson;
using UnityEngine;

/// <summary>
/// Статический класс, который создает сетевые сообщения в формате JSON.
/// </summary>
public static class MatchDataJson
{   
    /// <summary>
    /// Создает сетевое сообщение, содержащее скорость и местоположение.
    /// </summary>
    /// <param name="velocity">Скорость для отправки.</param>
    /// <param name="position">Позиция для отправки.</param>
    /// <returns>JSON строка с информацией о позиции и месте.</returns>
    public static string VelocityAndPosition(Vector3 velocity, Vector3 position)
    {
        var values = new Dictionary<string, string>
        {
            { "velocity.x", velocity.x.ToString() },
            { "velocity.y", velocity.y.ToString() },
            { "velocity.z", velocity.z.ToString() },
            { "position.x", position.x.ToString() },
            { "position.y", position.y.ToString() },
            { "position.z", position.z.ToString() }
        };

        return values.ToJson();
    }
    
    /// <summary>
    /// Создает сетевое сообщение, содержащее вводимые игроком данные.
    /// </summary>
    /// <param name="horizontalInput">The current horizontal input.</param>
    /// <param name="jump">The jump input.</param>
    /// <param name="attack">The attack input.</param>
    /// <param name="weaponid">ID of weapon.</param>
    /// <returns>JSON строка с инпутом игрока.</returns>
    public static string Input(float horizontalInput, bool jump, bool jumpOff, bool attack, bool attackHeld, float weaponid)
    {
        var values = new Dictionary<string, string>
        {
            { "horizontalInput", horizontalInput.ToString() },
            { "jump", jump.ToString() },
            { "jumpOff", jumpOff.ToString() },
            { "attack", attack.ToString() },
            { "attackHeld", attackHeld.ToString() },
            { "id", weaponid.ToString() }
        };

        return values.ToJson();
    }

    /// <summary>
    /// Создает сетевое сообщение, в котором указывается, что игрок умер, и позиция, в которой он умер.
    /// </summary>
    /// <param name="position">The position on death.</param>
    /// <returns>A JSONified string containing the player's position on death.</returns>
    public static string Died(Vector3 position)
    {
        var values = new Dictionary<string, string>
        {
            { "position.x", position.x.ToString() },
            { "position.y", position.y.ToString() },
            { "position.z", position.z.ToString() }
        };

        return values.ToJson();
    }

    /// <summary>
    /// Создает сетевое сообщение, указывающее, что игрок возродился и в какой точке возрождения.
    /// </summary>
    /// <param name="spawnIndex">The spawn point.</param>
    /// <returns>A JSONified string containing the player's respawn point.</returns>
    public static string Respawned(int spawnIndex)
    {
        var values = new Dictionary<string, string>
        {
            { "spawnIndex", spawnIndex.ToString() },
        };

        return values.ToJson();
    }

    public static string LootSpawned(Vector3 position)
    {
        var values = new Dictionary<string, string>
        {
            { "loot.x", position.x.ToString() },
            { "loot.y", position.y.ToString() },
            { "loot.z", position.z.ToString() }
        };

        return values.ToJson();
    }

    /// <summary>
    /// Создает сетевое сообщение, указывающее, что должен начаться новый раунд и кто выиграл предыдущий раунд.
    /// </summary>
    /// <param name="winnerPlayerName">The winning player's name.</param>
    /// <returns>A JSONified string containing the winning players name.</returns>
    public static string StartNewRound(string winnerPlayerName)
    {
        var values = new Dictionary<string, string>
        {
            { "winningPlayerName", winnerPlayerName }
        };
        
        return values.ToJson();
    }
}
