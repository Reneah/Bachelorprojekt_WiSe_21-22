using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DA.Menu
{
    public class ChangeOptionsColor : MonoBehaviour
    {
        [Tooltip("the different options header images for the colors")]
       [SerializeField] private GameObject[] _buttonColorImage;

        public void OnClickAudio()
        {
            _buttonColorImage[0].SetActive(true);
            _buttonColorImage[1].SetActive(false);
            _buttonColorImage[2].SetActive(false);
            _buttonColorImage[3].SetActive(false);
        }

        public void OnClickGraphics()
        {
            _buttonColorImage[0].SetActive(false);
            _buttonColorImage[1].SetActive(true);
            _buttonColorImage[2].SetActive(false);
            _buttonColorImage[3].SetActive(false);
        }

        public void OnClickGame()
        {
            _buttonColorImage[0].SetActive(false);
            _buttonColorImage[1].SetActive(false);
            _buttonColorImage[2].SetActive(true);
            _buttonColorImage[3].SetActive(false);
        }

        public void OnClickControls()
        {
            _buttonColorImage[0].SetActive(false);
            _buttonColorImage[1].SetActive(false);
            _buttonColorImage[2].SetActive(false);
            _buttonColorImage[3].SetActive(true);
        }
    }

}
