using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class UserData : MonoBehaviour
{
    public Player Player => _player;
    private Player _player;

    public void SetPlayer(Player player)
    {
        _player = player;
    }
}
