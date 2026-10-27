using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[Serializable]
public class SpawnCharsData
{
    public int spawnCount;
    public bool isRandomPositionXZ;
    public float xRange;
    public float zRange;
}


public class DLCCharacterAuthoring : MonoBehaviour {
    public string CharacterID;

    public SpawnCharsData CharacterData;
    void Start() {
        World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<CharacterLoadSystem>()
            .CustomScheduleCharacterLoad(CharacterID,CharacterData);
    }
}