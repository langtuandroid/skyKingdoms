using SO;
using UnityEngine;
using Utils;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private PlayerSO       _playerSO;
    [SerializeField] private GameObject     _playerPrefabBoy;
    [SerializeField] private GameObject     _playerPrefabGirl;
    
    public delegate void OnPlayerInstantiated(Transform playerTransform);
    
    public static event OnPlayerInstantiated PlayerInstantiated;

    public PlayerSO PlayerSO
    {
        get => _playerSO;
        set => _playerSO = value;
    }

    public void PlayerInstantation(Transform playerInitialPosition = null)
    {
        Vector3 playerPosition = Vector3.zero;
        if (playerInitialPosition != null)
            playerPosition = playerInitialPosition.position;
        
        if(_playerSO.player == null)
            Instantiate( _playerPrefabBoy, playerPosition, Quaternion.identity );
        else
        {
            if (_playerSO.player == "B")
            {
                Constants.Character = "Leo";
                GameObject Leo = Instantiate( _playerPrefabBoy, playerPosition, Quaternion.identity );
                PlayerInstantiated?.Invoke(Leo.transform);
            }
            else if (_playerSO.player == "G")
            {
                Constants.Character = "Magen";
                GameObject Magen = Instantiate( _playerPrefabGirl, playerPosition, Quaternion.identity );
                PlayerInstantiated?.Invoke(Magen.transform);
            }
        }
    }
}
