using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform player_position;
    private void Awake()
    {
        player_position = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Transform>();
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 1;
    }
}
