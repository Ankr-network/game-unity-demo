using System;
using System.Collections;
using System.Collections.Generic;
using MirageSDK.WalletConnectSharp.Unity;
using UnityEngine;

namespace MirageSDK.UseCases
{
    public class UseCaseIndex : MonoBehaviour
    {

        [SerializeField]
        private GameObject _loginButton;
        [SerializeField]
        private GameObject _useCaseButtonsMenu;

        void Start()
        {
           /* if(WalletConnect.IsSessionSaved()) 
                EnableUseCaseMenu();*/
            
        }

        private void EnableUseCaseMenu() {
            _loginButton.SetActive(false);
            _useCaseButtonsMenu.SetActive(true);
        }

    }

}
