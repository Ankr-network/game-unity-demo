using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WalletConnectSharp.Core;
using WalletConnectSharp.Unity;

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
            if(WalletConnect.IsSessionSaved()) 
                EnableUseCaseMenu();
            
        }

        private void EnableUseCaseMenu() {
            _loginButton.SetActive(false);
            _useCaseButtonsMenu.SetActive(true);
        }

    }

}
