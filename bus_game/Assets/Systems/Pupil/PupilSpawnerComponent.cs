﻿using SystemBase.Core.Components;
using UnityEngine;

namespace Systems.Pupil
{
    public class PupilSpawnerComponent : GameComponent
    {
        public GameObject pupilPrefab;
        public float spawnSpeed = 10f;
        public bool IsSpawning { get; set; } = false;

        public Sprite[] sprites;
    }
}