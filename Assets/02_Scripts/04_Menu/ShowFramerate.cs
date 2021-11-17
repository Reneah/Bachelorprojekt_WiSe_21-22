using UnityEngine;
using System.Collections;

// It is not my script : http://wiki.unity3d.com/index.php?title=FramesPerSecond&_ga=2.209546843.832596759.1589348256-2109853351.1584380006
// I just explain it

namespace DA.Menu
{
    public class ShowFramerate : MonoBehaviour
    {
        //
        float deltaTime = 0.0f;

        void Update()
        {
            //
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        /// <summary>
        /// calculate the frames, create the label and show them on the screen
        /// </summary>
        void OnGUI()
        {
            // get the scene width and high
            int w = Screen.width, h = Screen.height;

            // create a new GUI Style
            GUIStyle style = new GUIStyle();

            // create a new rect 
            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            // set the rect to the upper right corner
            style.alignment = TextAnchor.UpperRight;
            // set the font size
            style.fontSize = h * 2 / 100;
            // set the color of the text
            style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
            // calculate the milliseconds between the frames
            float msec = deltaTime * 1000.0f;
            //calculates the fps per second
            float fps = 1.0f / deltaTime;
            // show the text on the screen
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            // create teh label for the text
            GUI.Label(rect, text, style);
        }
    }
}

