using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WXFontContrl : MonoBehaviour
{
    Font wxFont;
    // Start is called before the first frame update
    void Start()
    {
        WeChatWASM.WX.GetWXFont("", (font) =>
        {
            wxFont = font;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
