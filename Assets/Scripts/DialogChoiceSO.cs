using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog choice", menuName = "Dialog System/Dialog Choice")]
public class DialogChoiceSO : ScriptableObject
{
    public string text;
    public int nextId;
}
