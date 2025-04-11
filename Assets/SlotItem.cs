using UnityEngine;
using UnityEngine.UI;

public class SlotItem : MonoBehaviour
{
    private TestItem _item;
    public Image _image;

    public void Init(TestItem item)
    {
        _item = item;
        _image.sprite = item._Icon;
        
    }

}
