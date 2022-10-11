using UnityEngine;
using System.Collections;
using kcp2k;
using Mirror;

namespace GameStudio.HunterGatherer.Networking.UI
{
    /// <summary>
    /// Handles the logging in of the user by providing options for username and roomname to connect and join a room with
    /// </summary>
    public class LoginFormHandler : MonoBehaviour
    {
        #region Static Fields
        public static string s_Username;
        #endregion

        #region Serialized Fields
        [Header("References")]
        [SerializeField, Tooltip("In milliseconds")] private float maxConnectionTime = 5000f;
        #endregion

        #region private fields
        private int defaultTimeout;
        private Coroutine ConnectionCoroutine;
        #endregion

        /// <summary>
        /// Handles the opening of the login form.
        /// </summary>
        public void Open()
        {
            defaultTimeout = NetworkRoomManager.Instance.GetComponent<KcpTransport>().Timeout;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Sets the static username to the username in the input field.
        /// </summary>
        /// <param name="newValue">The new value for username</param>
        public void OnUsernameEdit(string newValue)
        {
            if (!newValue.Contains(" "))
            {
                s_Username = newValue;                
            }
        }

        /// <summary>
        /// Sets the new serveraddres and disables the client if it was already connected or was connecting.
        /// </summary>
        /// <param name="newValue">The new value </param>
        public void OnServerAddresEdit(string newValue)
        {
            if (!newValue.Contains(" "))
            {
                NetworkRoomManager.Instance.networkAddress = newValue;
                NetworkRoomManager.Instance.GetComponent<KcpTransport>().ClientDisconnect();
                NetworkRoomManager.Instance.GetComponent<KcpTransport>().Timeout = defaultTimeout;
                NetworkRoomManager.Instance.StopClient();
            }
        }

        /// <summary>
        /// The behaviour when the startbutton is clicked 
        /// </summary>
        public void OnStartButtonClick()
        {
            if(string.IsNullOrEmpty(s_Username))
            {
                //TODO: give feedback message to player
                Debug.LogError("Username was not set!");
                return;
            }
            if(ConnectionCoroutine != null) return;
            NetworkRoomManager.Instance.GetComponent<KcpTransport>().Timeout = Mathf.CeilToInt(maxConnectionTime);
            NetworkRoomManager.Instance.StartClient();
            ConnectionCoroutine = StartCoroutine(CheckForConnection());
        }

        /// <summary>
        /// Checks if a connection is possible. 
        /// If a connection is not possible it stops the connection.
        /// </summary>
        private IEnumerator CheckForConnection()
        {
            float t = Time.time;
            while (!NetworkClient.isConnected && t + maxConnectionTime / 1000f >= Time.time)
            {
                yield return null;
            }
            if(!NetworkClient.isConnected)
            {
                NetworkRoomManager.Instance.GetComponent<KcpTransport>().ClientDisconnect();
                Debug.LogError("connection failed");              
                NetworkRoomManager.Instance.StopClient();
            }
            NetworkRoomManager.Instance.GetComponent<KcpTransport>().Timeout = defaultTimeout;
            ConnectionCoroutine = null;
        }
    }
}