using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer
{
    class EventManager
    {
        Dictionary<string, Action?> noParamEvents = new Dictionary<string, Action?>();
        Dictionary<string, Action<EventArgs>?> withParamEvents = new Dictionary<string, Action<EventArgs>?>();

        #region Thread-safe singleton setup
        static EventManager instance = null;
        static readonly object _lock = new object();

        public static EventManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new EventManager();
                    }
                    return instance;
                }
            }
        }
        #endregion

        #region Manage Events
        /// <summary>
        /// Creates a new empty event and adds it to one of the event dictionaries
        /// </summary>
        /// <param name="eventKey">The key that will be used to reference an event</param>
        /// <param name="hasParameters">Does the event pass parameters</param>
        public void AddEvent(string eventKey, bool hasParameters = false)
        {
            if (!hasParameters)
            {
                if (!instance.noParamEvents.ContainsKey(eventKey))
                    instance.noParamEvents.Add(eventKey, delegate { });
            }
            else
            {
                if (!instance.withParamEvents.ContainsKey(eventKey))
                    instance.withParamEvents.Add(eventKey, delegate { });
            }
        }

        /// <summary>
        /// Adds an existing event to the events dictionary
        /// </summary>
        /// <param name="eventKey">The key that will be used to reference an event</param>
        /// <param name="action">The existing event to add to the dictionary</param>
        public void AddEvent(string eventKey, Action action)
        {
            if (instance.noParamEvents.ContainsKey(eventKey))
            {
                instance.noParamEvents[eventKey] += action;
            }
            else
            {
                instance.noParamEvents.Add(eventKey, action);
            }
        }

        /// <summary>
        /// Adds an existing event with paramters to the events dictionary
        /// </summary>
        /// <param name="eventKey">The key that will be used to reference an event</param>
        /// <param name="action">The existing event to add to the dictionary</param>
        public void AddEvent(string eventKey, Action<EventArgs> action)
        {
            if (instance.withParamEvents.ContainsKey(eventKey))
            {
                instance.withParamEvents[eventKey] += action;
            }
            else
            {
                instance.withParamEvents.Add(eventKey, action);
            }
        }

        /// <summary>
        /// Remove an existing event from the dictionary
        /// </summary>
        /// <param name="eventKey"></param>
        public void RemoveEvent(string eventKey)
        {
            if (instance.noParamEvents.ContainsKey(eventKey))
            {
                instance.noParamEvents.Remove(eventKey);
            }
            else if (instance.withParamEvents.ContainsKey(eventKey))
            {
                instance.withParamEvents.Remove(eventKey);
            }
            else
            {
                Debug.LogError($"<b>[Event Manager]</b> Could not find event {eventKey}!");

            }
        }
        #endregion

        #region Manage Listeners
        /// <summary>
        /// Add a listener to an event, if the event doens't exist a new one will be created
        /// </summary>
        /// <param name="eventKey">The name of the event to subscribe to</param>
        /// <param name="method">The method that will be exectued when the event is invoked</param>
        public void AddListener(string eventKey, Action? method)
        {
            if (!instance.noParamEvents.ContainsKey(eventKey))
            {
                instance.noParamEvents.Add(eventKey, delegate { });
            }

            instance.noParamEvents[eventKey] += method;
        }

        /// <summary>
        /// Add a listener to an event, if the event doens't exist a new one will be created
        /// </summary>
        /// <param name="eventKey">The name of the event to subscribe to</param>
        /// <param name="method">The method that will be exectued when the event is invoked</param>
        public void AddListener(string eventKey, Action<EventArgs> method)
        {
            if (!instance.withParamEvents.ContainsKey(eventKey))
            {
                instance.withParamEvents.Add(eventKey, delegate { });
            }

            instance.withParamEvents[eventKey] += method;
        }

        /// <summary>
        /// Remove a listener from an event
        /// </summary>
        /// <param name="eventKey">The name of the event</param>
        /// <param name="method">The method which needs to be removed</param>
        public void RemoveListener(string eventKey, Action method)
        {
            if (!instance.noParamEvents.ContainsKey(eventKey))
            {
                Debug.LogError($"<b>[Event Manager]</b> Could not find event {eventKey}!");

            }

            instance.noParamEvents[eventKey] -= method;
        }

        /// <summary>
        /// Remove a listener from an event
        /// </summary>
        /// <param name="eventKey">The name of the event</param>
        /// <param name="method">The method which needs to be removed</param>
        public void RemoveListener(string eventKey, Action<EventArgs> method)
        {
            if (!instance.withParamEvents.ContainsKey(eventKey))
            {
                Debug.LogError($"<b>[Event Manager]</b> Could not find event {eventKey}!");
                return;

            }

            instance.withParamEvents[eventKey] -= method;
        }
        #endregion

        #region Execution
        /// <summary>
        /// Invoke the event and execute it's listeners
        /// </summary>
        /// <param name="eventKey">The event name</param>
        public void Invoke(string eventKey)
        {
            if (instance.noParamEvents.ContainsKey(eventKey))
            {
                instance.noParamEvents[eventKey]?.Invoke();
            }
            else
            {
                Debug.LogError($"<b>[Event Manager]</b> Could not find event {eventKey}!");
            }
        }

        /// <summary>
        /// Invoke the event and execute it's listeners
        /// </summary>
        /// <param name="eventKey">The event name</param>
        /// <param name="args">The arguments to use</param>
        public void Invoke(string eventKey, EventArgs args)
        {
            if (instance.withParamEvents.ContainsKey(eventKey))
            {
                instance.withParamEvents[eventKey]?.Invoke(args);
            }
            else
            {
                Debug.LogError($"<b>[Event Manager]</b> Could not find event {eventKey}!");

            }
        }
        #endregion

        #region Ohter
        /// <summary>
        /// Get an event (you won't be able to add or remove events using this!)
        /// </summary>
        /// <param name="eventKey">The name of the event</param>
        /// <returns>An event</returns>
        public Action? GetEvent(string eventKey)
        {
            if (instance.noParamEvents.ContainsKey(eventKey))
            {
                return instance.noParamEvents[eventKey];
            }
            else
            {
                Debug.LogError($"<b>[Event Manager]</b> Could not find event {eventKey}!");
                return null;
            }
        }

        /// <summary>
        /// Get an event (you won't be able to add or remove events using this!)
        /// </summary>
        /// <param name="eventKey">The name of the event</param>
        /// <returns>An event</returns>
        public Action<EventArgs>? GetParameterizedEvent(string eventKey)
        {
            if (instance.withParamEvents.ContainsKey(eventKey))
            {
                return instance.withParamEvents[eventKey];
            }
            else
            {
                Debug.LogError($"<b>[Event Manager]</b> Could not find event {eventKey}!");
                return null;
            }
        }
        #endregion
    }
}
