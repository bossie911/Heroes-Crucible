using GameStudio.HunterGatherer.Divisions;
using System;

namespace GameStudio.HunterGatherer.Networking
{
    public static class NetworkEvents
    {
        public static event Action OnClientDisconnect;
        
        public static void ClientDisconnect()
        {
            OnClientDisconnect?.Invoke();
        }

        public static event Action OnRoomClientEnter;

        public static void RoomClientEnter()
        {
            OnRoomClientEnter?.Invoke();
        }

        public static event Action<Division> OnDivisionCreated;

        public static void DivisionCreated(Division division)
        {
            OnDivisionCreated?.Invoke(division);
        }

        public static event Action<int> OnAddDivisionItem;

        public static void AddDivisionItem(int amount)
        {
            OnAddDivisionItem?.Invoke(amount);
        }
        
        public static event Action OnStartMatch;

        public static void StartMatch()
        {
            OnStartMatch?.Invoke();
        }

        public static event Action<int> OnHeroDeath;

        public static void HeroDeath(int PlayerID)
        {
            OnHeroDeath?.Invoke(PlayerID);
        }
        public static event Action OnAllPlayersEnteredScene;

        public static void AllPlayersEnteredScene()
        {
            OnAllPlayersEnteredScene?.Invoke();
        }
    }
}