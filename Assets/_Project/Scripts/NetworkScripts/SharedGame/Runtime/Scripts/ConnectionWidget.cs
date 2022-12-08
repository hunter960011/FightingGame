﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace SharedGame
{

    public class ConnectionWidget : MonoBehaviour
    {
        public TMP_InputField[] inpIps;
        public Toggle[] tglSpectators;
        public TMP_InputField inpPlayerIndex;
        public Toggle tgLocal;
        public Button btnConnect;

        private GameManager gameManager => GameManager.Instance;

        private void Awake()
        {
            gameManager.OnRunningChanged += OnRunningChanged;
            btnConnect.onClick.AddListener(OnConnect);

            var connections = new List<Connections>();
            connections.Add(new Connections()
            {
                ip = "127.0.0.1",
                port = 7000,
                spectator = false,
            });
            connections.Add(new Connections()
            {
                ip = "127.0.0.1",
                port = 7001,
                spectator = false,
            });
            inpPlayerIndex.text = "0";
            LoadConnectionInfo(connections);
        }

        private void OnConnect()
        {
            if (tgLocal.isOn)
            {
                NetworkInput.IS_LOCAL = true;
                gameManager.StartLocalGame();
            }
            else
            {
                NetworkInput.IS_LOCAL = false;
                var connectionInfo = GetConnectionInfo();
                var perf = FindObjectOfType<GgpoPerformancePanel>();
                perf.Setup();
                var playerIndex = int.Parse(inpPlayerIndex.text);
                gameManager.StartGGPOGame(perf, connectionInfo, playerIndex);
            }
        }

        public void StartGGPO(string ipOne, string ipTwo, int index)
        {
            var connections = new List<Connections>();
            connections.Add(new Connections()
            {
                ip = ipOne,
                port = 7000,
                spectator = false,
            });
            connections.Add(new Connections()
            {
                ip = ipTwo,
                port = 7001,
                spectator = false,
            });
            LoadConnectionInfo(connections);
            NetworkInput.IS_LOCAL = false;
            var connectionInfo = GetConnectionInfo();
            var perf = FindObjectOfType<GgpoPerformancePanel>();
            perf.Setup();
            var playerIndex = index;
            gameManager.StartGGPOGame(perf, connectionInfo, playerIndex);
        }

        private void OnDestroy()
        {
            if (gameManager != null)
            {
                gameManager.OnRunningChanged -= OnRunningChanged;
            }
            btnConnect.onClick.RemoveListener(OnConnect);
        }

        private void OnRunningChanged(bool obj)
        {
            gameObject.SetActive(!obj);
        }

        public void LoadConnectionInfo(IList<Connections> connections)
        {
            for (int i = 0; i < connections.Count; ++i)
            {
                inpIps[i].text = connections[i].ip + ":" + connections[i].port;
                tglSpectators[i].isOn = connections[i].spectator;
            }
        }

        public IList<Connections> GetConnectionInfo()
        {
            var connections = new List<Connections>(inpIps.Length);
            for (int i = 0; i < inpIps.Length; ++i)
            {
                var split = inpIps[i].text.Split(':');
                connections.Add(new Connections()
                {
                    ip = split[0],
                    port = ushort.Parse(split[1]),
                    spectator = false,
                });
            }
            return connections;
        }
    }
}