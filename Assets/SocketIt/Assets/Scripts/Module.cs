﻿using UnityEngine;
using System.Collections.Generic;

namespace SocketIt
{
    [AddComponentMenu("SocketIt/Module/Module")]
    public class Module : MonoBehaviour
    {
        public delegate void ModuleEvent(Connection connection);
        public event ModuleEvent OnConnect;
        public event ModuleEvent OnDisconnect;

        private List<Socket> Sockets = new List<Socket>();

        private List<Connection> connectedModules = new List<Connection>();

        public List<Connection> ConnectedModules
        {
            get
            {
                return connectedModules;
            }
        }

        public void Awake()
        {
            Sockets = new List<Socket>(GetComponentsInChildren<Socket>());
        }
			
        public void Reset()
        {
            Socket[] sockets = GetComponentsInChildren<Socket>();
            foreach(Socket socket in sockets)
            {
                Sockets.Add(socket);
                if(socket.Module == null)
                {
                    socket.Module = this;
                }
            }
        }

        /**
         * Makes sure that all current connections get updated when this gameObject gets destroyed
         */
        public void OnDestroy()
        {
            foreach (Connection connection in connectedModules)
            {
                connection.SocketB.Module.OnOtherModuleDestroyed(this);
            }
        }

        /**
         * Gets called whenever a currently connected or possible connected module gets destroyed.
         * This is neccessary to cleanup references to the destroyed module and its sockets
         */ 
        public void OnOtherModuleDestroyed(Module destoryedModule)
        {
            Connection connection = GetConnection(destoryedModule);
            if (connection != null)
            {
                RemoveConnection(connection.SocketA, connection.SocketB, connection.SocketB);
                connection.SocketA.Clear();
            }
        }

        /**
         * Gets called when one of the modules sockets gets destroyed. 
         */ 
        public void OnSocketDestroyed(Socket destroyedSocket)
        {
            RemoveSocket(destroyedSocket); 
        }

        /**
          * Gets called when a socket of a connected module gets destroyed. 
          * We need this to cleanup references to this socket and remove connections
          */
        public void OnOtherSocketDestroyed(Socket destroyedSocket)
        {
            Connection connection = GetConnection(destroyedSocket);
            if(connection != null)
            {
                RemoveConnection(connection.SocketA, connection.SocketB, connection.SocketB);
            }
        }

        public void OnSocketConnect(Socket ownSocket, Socket otherSocket, Socket initiator)
        {
            AddConnection(ownSocket, otherSocket, initiator);
        }

        public void OnSocketDisconnect(Socket ownSocket, Socket otherSocket, Socket initiator)
        {
            RemoveConnection(ownSocket, otherSocket, initiator);
        }

        public void DisconnectAll()
        {
            List<Connection> connections = new List<Connection>(connectedModules);
            foreach(Connection connection in connections)
            {
                connection.SocketA.Disconnect(connection.SocketB);
            }
        }

        public void Disconnect(Module module)
        {
            Connection connection = GetConnection(module);
            if(connection != null)
            {
                connection.SocketA.Disconnect(connection.SocketB);
            }
        }

        /**
         * Removes the socket reference from the module.
         */
        public void RemoveSocket(Socket socketToRemove)
        {
            if (!Sockets.Contains(socketToRemove))
            {
                return;
            }

            Sockets.Remove(socketToRemove);

            Connection connection = GetConnection(socketToRemove);

            if(connection != null)
            {
                connection.SocketB.Module.OnOtherSocketDestroyed(socketToRemove);
                RemoveConnection(connection.SocketA, connection.SocketB, connection.SocketB);
            }
        }

        private void AddConnection(Socket callingSocket, Socket otherSocket, Socket initiator)
        {
			Connection oldConnection = GetConnection (otherSocket.Module);
			if(oldConnection != null){
				Debug.LogWarningFormat ("Module {0} already connected to module {1}", callingSocket.Module.name, otherSocket.Module.name);
				return;
			}	

			Connection connection = gameObject.AddComponent<Connection> ();
			connection.SocketA = callingSocket;
			connection.SocketB = otherSocket; 
			connection.Initiator = initiator;
            connectedModules.Add(connection);

            if (OnConnect != null)
            {
                OnConnect(connection);
            }
        }

        private void RemoveConnection(Socket callingSocket, Socket otherSocket, Socket initiator)
        {
            Connection connection = GetConnection(otherSocket);
            if (connection == null)
            {
                return;
            }

            connectedModules.Remove(connection);
			Destroy (connection);
            
            if (OnDisconnect != null)
            {
                OnDisconnect(connection);
            }
        }

        public Connection GetConnection(Module module)
        {
            foreach (Connection connection in connectedModules)
            {
                if (module == connection.SocketB.Module)
                {
                    return connection;
                }
            }

            return null;
        }

        public Connection GetConnection(Socket socket)
        {
            foreach (Connection connection in connectedModules)
            {
                if (socket == connection.SocketB || socket == connection.SocketA)
                {
                    return connection;
                }
            }

            return null;
        }

		public Connection GetConnection(Connection con)
		{
			foreach (Connection connection in connectedModules)
			{
				if (con.SocketB == connection.SocketB && con.SocketA == connection.SocketA)
				{
					return connection;
				}
			}

			return null;
		}
    }
}
