﻿using UnityEngine;
using System.Collections;
using System;

namespace SocketIt {
    [RequireComponent(typeof(SnapModule))]
    [AddComponentMenu("SocketIt/Snap Validators/Free Validator")]
    public class FreeValidator : MonoBehaviour, ISnapValidator
	{
        public bool SnapOnlyFreeSockets = true;

	    public bool Validate(Snap snap)
	    {
            if (SnapOnlyFreeSockets && !BothSocketsAreFree(snap))
            {
                return false;
            }

            return true;
	    }

        private bool BothSocketsAreFree(Snap snap)
        {
            return !snap.SocketA.Socket.IsConnected && !snap.SocketB.Socket.IsConnected;

        }
    }
}