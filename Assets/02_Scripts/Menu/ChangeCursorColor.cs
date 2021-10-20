using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DA.Menu
{
    public class ChangeCursorColor : MonoBehaviour
    {
        public void OnMouseOver()
        {
            Cursor.SetCursor(FindObjectOfType<Menu>().CursorTexture[1], Vector2.down, CursorMode.Auto);
        }

        public void OnMouseExit()
        {
            Cursor.SetCursor(FindObjectOfType<Menu>().CursorTexture[0], Vector2.down, CursorMode.Auto);
        }
    }
}

