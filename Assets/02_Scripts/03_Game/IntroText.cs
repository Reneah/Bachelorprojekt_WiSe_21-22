using UnityEngine;


[CreateAssetMenu(fileName = "NewScriptableAudioObject", menuName = "Scriptable Objects/Intro Text", order = -50)]
public class IntroText : ScriptableObject
{
    [Tooltip("the time how long the text should appears when the fade in is completed")]
    public float fadeStayTime;
    [TextArea(2,200)]
    public string text;
}
