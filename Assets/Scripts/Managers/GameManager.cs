using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public SpawnManager SpawnManager;
    public Action<WorldObject> OnObjectDeath;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(SpawnManager);
            SpawnManager.Initialize();
            OnObjectDeath += OnObjectDeathCall;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    private void OnObjectDeathCall(WorldObject obj)
    {
        SpawnManager.TakeBackObject(obj);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            SpawnManager.SpawnBullet(pos, new SpawnData(0, 1, ObjectType.Bullet, "bullet", 1)); 
        }
    }
}
