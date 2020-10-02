using System;
using System.Collections.Generic;

namespace Signals
{
    public abstract class Signal
    {
    }

    /// <summary>
    /// Signal Dispatcher Type Safe based on Will R. Miller. (http://www.willrmiller.com/?p=87)
    /// </summary>
    public class SignalDispatcher
    {
        private struct DelegatePair
        {
            public Type Type;
            public SignalDelegate Delegate;
        }

        public delegate void SignalDelegate<T> (T e) where T : Signal;
        private delegate void SignalDelegate (Signal e);

        private readonly Dictionary<Type, List<SignalDelegate>> delegates;
        private readonly Dictionary<Delegate, List<DelegatePair>> delegateCache;
        private readonly Dictionary<Type, bool> delegatesLock;

        public SignalDispatcher()
        {
            delegates = new Dictionary<Type, List<SignalDelegate>>();
            delegateCache = new Dictionary<Delegate, List<DelegatePair>>();
            delegatesLock = new Dictionary<Type, bool>();
        }

        ~SignalDispatcher()
        {
            Clear();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="del">SignalDelegate listener to add.</param>
        /// <typeparam name="T">Must inherit from Signal class.</typeparam>
        public void AddListener<T>(SignalDelegate<T> del) where T : Signal
        {
            if (!delegateCache.ContainsKey(del))
            {
                delegateCache[del] = new List<DelegatePair>();
            }

            var delegateType = typeof(T);
            if (!delegates.ContainsKey(delegateType))
            {
                delegates[delegateType] = new List<SignalDelegate>();
            }

            //Do not add delegate if it is already subscribed
            SignalDelegate internalDelegate;
            if (TryGetInternalDelegate(delegateType, del, out internalDelegate))
            {
                return;
            }

            // Create a new non-generic delegate which calls our generic one.
            // This is the delegate we actually invoke.
            internalDelegate = e => del((T)e);
            var delegatePair = new DelegatePair
            {
                Type = delegateType,
                Delegate = internalDelegate
            };
            delegateCache[del].Add(delegatePair);
            delegates[delegateType].Add(internalDelegate);
        }

        /// <summary>
        /// Remove a listener from an signal type.
        /// </summary>
        /// <param name="del">SignalDelegate listener to remove.</param>
        /// <typeparam name="T">Must inherit from Signal class.</typeparam>
        public void RemoveListener<T>(SignalDelegate<T> del) where T : Signal
        {
            var delegateType = typeof(T);
            SignalDelegate internalDelegate;
            if (!TryGetInternalDelegate(delegateType, del, out internalDelegate))
            {
                return;
            }

            if (delegates.ContainsKey(delegateType))
            {
                var i = delegates[delegateType].IndexOf(internalDelegate);
                if (i >= 0)
                {
                    delegates[delegateType][i] = null;
                }
                RemoveInternalDelegate(delegateType, del);

                RemoveAllNull(delegateType);
            }
        }

        /// <summary>
        /// Dispatch a signal.
        /// </summary>
        /// <param name="dispatchSignal">Instance of signal.</param>
        public void Dispatch(Signal dispatchSignal)
        {
            var type = dispatchSignal.GetType();
            Dispatch(dispatchSignal, type);
        }

        /// <summary>
        /// Dispatch a signal type without instantiating a signal object.
        /// </summary>
        /// <typeparam name="T">Must inherit from Signal class.</typeparam>
        public void Dispatch<T>() where T : Signal
        {
            var type = typeof(T);
            Dispatch(null, type);
        }

        /// <summary>
        /// Check if signal manager has listeners for a type of signal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasListeners<T>()
        {
            var type = typeof(T);
            return delegates.ContainsKey(type) && delegates[type].Count > 0;
        }

        /// <summary>
        /// Clear all signal listeners.
        /// </summary>
        public void Clear()
        {
            delegates.Clear();
            delegateCache.Clear();
        }

        private void Dispatch(Signal dispatchSignal, Type type)
        {
            if (delegates.ContainsKey(type))
            {
                delegatesLock[type] = true;

                for (var i = 0; i < delegates[type].Count; i++)
                {
                    var signalDelegate = delegates[type][i];
                    if (signalDelegate != null)
                    {
                        signalDelegate.Invoke(dispatchSignal);
                    }
                }

                delegatesLock[type] = false;

                RemoveAllNull(type);
            }
        }

        private void RemoveInternalDelegate(Type type, Delegate del)
        {
            if (!delegateCache.ContainsKey(del))
            {
                return;
            }

            bool found = false;
            for (int i = 0; i < delegateCache[del].Count && !found; i++)
            {
                var pair = delegateCache[del][i];
                if (pair.Type == type)
                {
                    delegateCache[del].RemoveAt(i);
                    found = true;
                }
            }

            if (found && delegateCache[del].Count == 0)
            {
                delegateCache.Remove(del);
            }
        }

        private bool TryGetInternalDelegate(Type type, Delegate del, out SignalDelegate internalDelegate)
        {
            internalDelegate = null;
            if (!delegateCache.ContainsKey(del))
            {
                return false;
            }

            for (var i = 0; i < delegateCache[del].Count; i++)
            {
                var pair = delegateCache[del][i];
                if (pair.Type == type)
                {
                    internalDelegate = pair.Delegate;
                    return true;
                }
            }

            return false;
        }

        private bool IsDelegateListLocked(Type type)
        {
            if (!delegatesLock.ContainsKey(type))
            {
                return false;
            }

            return delegatesLock[type];
        }

        private void RemoveAllNull(Type type)
        {
            if (IsDelegateListLocked(type))
            {
                return;
            }

            if (!delegates.ContainsKey(type))
            {
                return;
            }

            delegates[type].RemoveAll(IsNull);

            if (delegates[type].Count == 0)
            {
                delegates.Remove(type);
            }
        }

        private bool IsNull(object o)
        {
            return o == null;
        }

    }
}