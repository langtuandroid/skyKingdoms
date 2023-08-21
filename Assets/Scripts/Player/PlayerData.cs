using SO;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private PlayerSO       _playerSO;
    [SerializeField] private GameObject     _playerPrefabBoy;
    [SerializeField] private GameObject     _playerPrefabGirl;

    public PlayerSO PlayerSO
    {
        get => _playerSO;
        set => _playerSO = value;
    }

    public void PlayerInstantation(Transform playerInitialPosition = null)
    {
        if(playerInitialPosition == null) 
            playerInitialPosition.position = Vector3.zero;
        
        if(_playerSO.player == "B")
            Instantiate( _playerPrefabBoy, playerInitialPosition.position, Quaternion.identity );
        else if (_playerSO.player == "G")
            Instantiate( _playerPrefabGirl, playerInitialPosition.position, Quaternion.identity );
    }
}
