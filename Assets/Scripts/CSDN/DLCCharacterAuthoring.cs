using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DLCCharacterAuthoring : MonoBehaviour {
    public string CharacterID;
    
    void Start() {
        World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<CharacterLoadSystem>()
            .ScheduleCharacterLoad(CharacterID);
    }
}