using UnityEngine;

[CreateAssetMenu(fileName = "AlphabetData", menuName = "SignLanguage/AlphabetData")]
public class AlphabetData : ScriptableObject {
    public string letterName;
    public RuntimeAnimatorController animatorController;
    public Sprite letterSprite;
}