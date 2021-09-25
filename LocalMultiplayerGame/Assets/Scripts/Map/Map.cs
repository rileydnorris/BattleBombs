using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField]
    private Vector2[] _playerSpawnPositions;

    public Vector2[] GetPlayerSpawnPositions()
    {
        return _playerSpawnPositions;
    }
}
