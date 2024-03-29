﻿using UnityEngine;
using System.Collections;
using System;

namespace SocketIt.Example07
{
    public class BaseConnector : MonoBehaviour, IBaseConnector
    {
        private Renderer rend;

        public void Awake()
        {
            rend = GetComponent<Renderer>();
        }

        public void Connect(BaseModule baseModule)
        {
            ChangeEmissionColor(Color.green);
        }

        public void Disconnect(BaseModule baseModule)
        {
            ChangeEmissionColor(Color.black);
        }

        private void ChangeEmissionColor(Color color)
        {
            Material mat = rend.material;
            mat.SetColor("_EmissionColor", color);
        }
    }

}

