using UnityEngine;

[CreateAssetMenu(fileName = "TestItem", menuName= "New TestItem")]
public class TestItem : ScriptableObject
{
    public string Name;
    public int Value;

    public Sprite _Icon;
}
