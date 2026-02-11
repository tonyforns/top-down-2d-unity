using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundModelSO", menuName = "Scriptable Objects/SoundModelSO")]
[Serializable]
public class SoundModelSO : ScriptableObject
{
    public enum SoundName
    {
        Hit,
        Heal,
        Death,
        ButtonClick,
        BackgroundMusic,
        PowerUp,
        LevelComplete,
        GameOver,
        CheckPoint,
        Jump,
        Interact,
        Attack,
        Coin,
        DoorOpen
    }
    
    public SoundName Name;
    public AudioClip Clip;
}
