using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace FirefighterFighter.Networking
{
    public class UserData : MonoBehaviour
    {
        public Player Player => _player;
        private Player _player;
        public int id;

        public void SetPlayer(Player player)
        {
            _player = player;
        }
    }
}