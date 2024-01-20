using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGG.Components.Menus
{
    public class ConfirmationScreen : MonoBehaviour
    {
        private DeleteProgressButton _deleteProgressButton;
        private DeleteProgressConfirm _confirmButton;

        private GameObject _viewport;

        private void Awake()
        {
            _deleteProgressButton = FindObjectOfType<DeleteProgressButton>();
            _confirmButton = FindObjectOfType<DeleteProgressConfirm>();

            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
        }

        private void Start()
        {
            _deleteProgressButton.OnDeleteProgress += Open;
            _confirmButton.OnConfirm += Close;
        }

        private void Open()
        {
            _viewport.SetActive(true);
        }

        public void Close()
        {
            _viewport.SetActive(false);
        }
    }
}
