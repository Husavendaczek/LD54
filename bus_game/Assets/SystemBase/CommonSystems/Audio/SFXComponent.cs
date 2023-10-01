using SystemBase.Core;
using SystemBase.Core.Components;
using UnityEngine;

namespace SystemBase.CommonSystems.Audio
{
    public class SFXComponent : GameComponent
    {
        [Range(0f, 2f)]
        public float MaxPitchChange = 0.25f;

        public SoundFile[] Sounds;
    }
}