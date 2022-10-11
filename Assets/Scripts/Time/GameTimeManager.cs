using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GameStudio.HunterGatherer.CustomEvents;
using Mirror;

namespace GameStudio.HunterGatherer.GameTime
{
    public class GameTimeManager : MonoBehaviour
    {
        [SerializeField]
        private static GameTimeManager instance;
        
        [Tooltip("Round Time in seconds")]
        [SerializeField] private List<float> roundTimes;
        [SerializeField] private List<TimeEvent> timeEvents = null;
        
        private int currentRound = 0;
        private int currentSecond;

        public static GameTimeManager Instance { get { return instance; } }
        
        /// <summary> The amount of time per round </summary>
        public List<float> RoundTimes => roundTimes;

        /// <summary> The current round number </summary>
        public int CurrentRound => currentRound;
        
        /// <summary> How much time is left in the round </summary>
        public float RoundTimeLeft { get; private set; }
        
        public List<TimeEvent> TimeEvents => timeEvents;
        
        public UnityEventInt OnSecondIncrement { get; } = new UnityEventInt();
        public UnityEvent OnRoundIncrement { get; } = new UnityEvent();

        public bool isEnabled = false;
        
        /// <summary> Value representing the current second as an integer, chosen because an int comparison is easier than a float comparison </summary>
        public int CurrentSecond
        {
            get
            {
                return currentSecond;
            }
            set
            {
                if (currentSecond == value)
                {
                    return;
                }

                currentSecond = value;
                OnSecondIncrement.Invoke(value);
            }
        }
        
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            else
            {
                instance = this;
            }
            RoundTimeLeft = roundTimes[0];
        }
        
        private void OnEnable()
        {
            OnSecondIncrement.AddListener(CheckEventTime);
            OnRoundIncrement.AddListener(StartNewRound);
        }

        private void OnDisable()
        {
            OnSecondIncrement.RemoveListener(CheckEventTime);
            OnRoundIncrement.RemoveListener(StartNewRound);
        }

        private void Update()
        {
            if (isEnabled)
            {
                UpdateTime();
            }
        }
        
        /// <summary> Decreases the time per round, stops when 0 is reached for the clock </summary>
        private void UpdateTime()
        {
            if (RoundTimeLeft >= 0)
            {
                RoundTimeLeft -= Time.deltaTime;
                CurrentSecond = Mathf.RoundToInt(RoundTimeLeft);
            }
        }
        
        private void CheckEventTime(int second)
        {
            foreach (var timeEvent in timeEvents)
            {
                if (timeEvent.second == second)
                {
                    timeEvent.onTimeMatched.Invoke();
                }
            }
        }

        private void StartNewRound()
        {
            //Check if the current round is the last round
            if (currentRound == roundTimes.Count - 1)
            {
                Debug.Log("Last round has happened");
                return;
            }
            
            currentRound++;
            RoundTimeLeft = roundTimes[currentRound];
            
            Debug.Log("Round: " + currentRound + " has started and has " + Mathf.RoundToInt(RoundTimeLeft) + " seconds left");
        }
        
        public void StopGameTime()
        {
            isEnabled = false;
        }
    }
}
