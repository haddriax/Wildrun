using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using System;

namespace MenuNamespace
{
    public class CameraManager : MonoBehaviour
    {
        enum MenuType
        {
            Main, Garage, Option, Race, Multi, Credits
        }

        [Serializable]
        class Menu
        {
            public MenuType type;
            public CinemachineVirtualCamera vc;
            public Canvas canvas;
        }

        [SerializeField] List<Menu> menus;
        Menu currentMenu = null;

        void Start()
        {
            SetMenu(MenuType.Main);
        }

        private void SetMenu(MenuType _type)
        {
            if (currentMenu != null)
            {
                currentMenu.vc.m_Priority = 0;
                currentMenu.canvas.gameObject.SetActive(false);
            }

            currentMenu = menus.Find(x => x.type == _type);
            currentMenu.vc.m_Priority = 1;
            currentMenu.canvas.gameObject.SetActive(true);
        }

        public void GoToMainMenu()  => SetMenu(MenuType.Main);
        public void GoToGarage()    => SetMenu(MenuType.Garage);
        public void GoToRace()      => SetMenu(MenuType.Race);
    }
}