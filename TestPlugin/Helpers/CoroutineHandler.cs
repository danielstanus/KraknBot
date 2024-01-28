using System;
using UnityEngine;

namespace TestPlugin.Helpers
{
    public class CoroutineHandler : MonoBehaviour
    {
        private static CoroutineHandler _instance;
        private Action _delayedAction;

        public static CoroutineHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject("CoroutineHandler");
                    _instance = obj.AddComponent<CoroutineHandler>();
                    DontDestroyOnLoad(obj);
                }
                return _instance;
            }
        }

        public void ExecuteDelayed(float delay, Action action)
        {
            _delayedAction = action;
            Invoke("ExecuteDelayedAction", delay);
        }

        private void ExecuteDelayedAction()
        {
            _delayedAction?.Invoke();
            _delayedAction = null; // Clears the action after execution
        }

        public void CancelDelayedExecution()
        {
            CancelInvoke("ExecuteDelayedAction");
        }
    }
}