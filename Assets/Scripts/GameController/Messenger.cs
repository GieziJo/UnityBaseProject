/*
 * Advanced C# messenger by Ilya Suzdalnitski. V1.0
 * 
 * Based on Rod Hyde's "CSharpMessenger" and Magnus Wolffelt's "CSharpMessenger Extended".
 * 
 * Features:
 	* Prevents a MissingReferenceException because of a reference to a destroyed message handler.
 	* Option to log all messages
 	* Extensive error detection, preventing silent bugs
 * 
 * Usage examples:
 	1. Messenger.AddListener<GameObject>("prop collected", PropCollected);
 	   Messenger.Broadcast<GameObject>("prop collected", prop);
 	2. Messenger.AddListener<float>("speed changed", SpeedChanged);
 	   Messenger.Broadcast<float>("speed changed", 0.5f);
 * 
 * Messenger cleans up its evenTable automatically upon loading of a new level.
 * 
 * Don't forget that the messages that should survive the cleanup, should be marked with Messenger.MarkAsPermanent(string)
 * 
 */

//#define LOG_ALL_MESSAGES
//#define LOG_ADD_LISTENER
//#define LOG_BROADCAST_MESSAGE
//#define REQUIRE_LISTENER

using System;
using System.Collections.Generic;
using UnityEngine;

static internal class Messenger
{
	#region Internal variables

	//Disable the unused variable warning
#pragma warning disable 0414
	//Ensures that the MessengerHelper will be created automatically upon start of the game.
	static MessengerHelper messengerHelper = (new GameObject("MessengerHelper")).AddComponent<MessengerHelper>();
#pragma warning restore 0414

	static public Dictionary<EventName, Delegate> eventTable = new Dictionary<EventName, Delegate>();

	//Message handlers that should never be removed, regardless of calling Cleanup
	static public List<EventName> permanentMessages = new List<EventName>();
	#endregion
	#region Helper methods
	//Marks a certain message as permanent.
	static public void MarkAsPermanent(EventName eventType)
	{
#if LOG_ALL_MESSAGES
		Debug.Log("Messenger MarkAsPermanent \t\"" + eventType + "\"");
#endif

		permanentMessages.Add(eventType);
	}


	static public void Cleanup()
	{
#if LOG_ALL_MESSAGES
		Debug.Log("MESSENGER Cleanup. Make sure that none of necessary listeners are removed.");
#endif

		List<EventName> messagesToRemove = new List<EventName>();

		foreach (KeyValuePair<EventName, Delegate> pair in eventTable)
		{
			bool wasFound = false;

			foreach (EventName message in permanentMessages)
			{
				if (pair.Key == message)
				{
					wasFound = true;
					break;
				}
			}

			if (!wasFound)
				messagesToRemove.Add(pair.Key);
		}

		foreach (EventName message in messagesToRemove)
		{
			eventTable.Remove(message);
		}
	}

	static public void PrintEventTable()
	{
		Debug.Log("\t\t\t=== MESSENGER PrintEventTable ===");

		foreach (KeyValuePair<EventName, Delegate> pair in eventTable)
		{
			Debug.Log("\t\t\t" + pair.Key + "\t\t" + pair.Value);
		}

		Debug.Log("\n");
	}
	#endregion

	#region Message logging and exception throwing
	static public void OnListenerAdding(EventName eventType, Delegate listenerBeingAdded)
	{
#if LOG_ALL_MESSAGES || LOG_ADD_LISTENER
		Debug.Log("MESSENGER OnListenerAdding \t\"" + eventType + "\"\t{" + listenerBeingAdded.Target + " -> " + listenerBeingAdded.Method + "}");
#endif

		if (!eventTable.ContainsKey(eventType))
		{
			eventTable.Add(eventType, null);
		}

		Delegate d = eventTable[eventType];
		if (d != null && d.GetType() != listenerBeingAdded.GetType())
		{
			throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
		}
	}

	static public void OnListenerRemoving(EventName eventType, Delegate listenerBeingRemoved)
	{
#if LOG_ALL_MESSAGES
		Debug.Log("MESSENGER OnListenerRemoving \t\"" + eventType + "\"\t{" + listenerBeingRemoved.Target + " -> " + listenerBeingRemoved.Method + "}");
#endif

		if (eventTable.ContainsKey(eventType))
		{
			Delegate d = eventTable[eventType];

			if (d == null)
			{
				throw new ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
			}
			else if (d.GetType() != listenerBeingRemoved.GetType())
			{
				throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
			}
		}
		else
		{
			throw new ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
		}
	}

	static public void OnListenerRemoved(EventName eventType)
	{
		if (eventTable[eventType] == null)
		{
			eventTable.Remove(eventType);
		}
	}

	static public void OnBroadcasting(EventName eventType)
	{
#if REQUIRE_LISTENER
		if (!eventTable.ContainsKey(eventType))
		{
			Debug.Log(string.Format("<color=red>Broadcasting message \"{0}\" but no listener found. Try marking the message with Messenger.MarkAsPermanent.</color>", eventType));
		}
#endif
	}

	static public BroadcastException CreateBroadcastSignatureException(EventName eventType)
	{
		return new BroadcastException(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
	}

	public class BroadcastException : Exception
	{
		public BroadcastException(string msg)
				: base(msg)
		{
		}
	}

	public class ListenerException : Exception
	{
		public ListenerException(string msg)
				: base(msg)
		{
		}
	}
	#endregion

	#region AddListener
	//No parameters
	static public void AddListener(EventName eventType, CallbackMessenger handler)
	{
		OnListenerAdding(eventType, handler);
		eventTable[eventType] = (CallbackMessenger)eventTable[eventType] + handler;
	}

	//Single parameter
	static public void AddListener<T>(EventName eventType, CallbackMessenger<T> handler)
	{
		OnListenerAdding(eventType, handler);
		eventTable[eventType] = (CallbackMessenger<T>)eventTable[eventType] + handler;
	}

	//Two parameters
	static public void AddListener<T, U>(EventName eventType, CallbackMessenger<T, U> handler)
	{
		OnListenerAdding(eventType, handler);
		eventTable[eventType] = (CallbackMessenger<T, U>)eventTable[eventType] + handler;
	}

	//Three parameters
	static public void AddListener<T, U, V>(EventName eventType, CallbackMessenger<T, U, V> handler)
	{
		OnListenerAdding(eventType, handler);
		eventTable[eventType] = (CallbackMessenger<T, U, V>)eventTable[eventType] + handler;
	}
	#endregion

	#region RemoveListener
	//No parameters
	static public void RemoveListener(EventName eventType, CallbackMessenger handler)
	{
		OnListenerRemoving(eventType, handler);
		eventTable[eventType] = (CallbackMessenger)eventTable[eventType] - handler;
		OnListenerRemoved(eventType);
	}

	//Single parameter
	static public void RemoveListener<T>(EventName eventType, CallbackMessenger<T> handler)
	{
		OnListenerRemoving(eventType, handler);
		eventTable[eventType] = (CallbackMessenger<T>)eventTable[eventType] - handler;
		OnListenerRemoved(eventType);
	}

	//Two parameters
	static public void RemoveListener<T, U>(EventName eventType, CallbackMessenger<T, U> handler)
	{
		OnListenerRemoving(eventType, handler);
		eventTable[eventType] = (CallbackMessenger<T, U>)eventTable[eventType] - handler;
		OnListenerRemoved(eventType);
	}

	//Three parameters
	static public void RemoveListener<T, U, V>(EventName eventType, CallbackMessenger<T, U, V> handler)
	{
		OnListenerRemoving(eventType, handler);
		eventTable[eventType] = (CallbackMessenger<T, U, V>)eventTable[eventType] - handler;
		OnListenerRemoved(eventType);
	}
	#endregion

	#region Broadcast
	//No parameters
	static public void Broadcast(EventName eventType)
	{
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
		OnBroadcasting(eventType);

		Delegate d;
		if (eventTable.TryGetValue(eventType, out d))
		{
			CallbackMessenger callback = d as CallbackMessenger;

			if (callback != null)
			{
				callback();
			}
			else
			{
				throw CreateBroadcastSignatureException(eventType);
			}
		}
	}

	//Single parameter
	static public void Broadcast<T>(EventName eventType, T arg1)
	{
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
		OnBroadcasting(eventType);

		Delegate d;
		if (eventTable.TryGetValue(eventType, out d))
		{
			CallbackMessenger<T> callback = d as CallbackMessenger<T>;

			if (callback != null)
			{
				callback(arg1);
			}
			else
			{
				throw CreateBroadcastSignatureException(eventType);
			}
		}
	}

	//Two parameters
	static public void Broadcast<T, U>(EventName eventType, T arg1, U arg2)
	{
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
		OnBroadcasting(eventType);

		Delegate d;
		if (eventTable.TryGetValue(eventType, out d))
		{
			CallbackMessenger<T, U> callback = d as CallbackMessenger<T, U>;

			if (callback != null)
			{
				callback(arg1, arg2);
			}
			else
			{
				throw CreateBroadcastSignatureException(eventType);
			}
		}
	}

	//Three parameters
	static public void Broadcast<T, U, V>(EventName eventType, T arg1, U arg2, V arg3)
	{
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
		OnBroadcasting(eventType);

		Delegate d;
		if (eventTable.TryGetValue(eventType, out d))
		{
			CallbackMessenger<T, U, V> callback = d as CallbackMessenger<T, U, V>;

			if (callback != null)
			{
				callback(arg1, arg2, arg3);
			}
			else
			{
				throw CreateBroadcastSignatureException(eventType);
			}
		}
	}
	#endregion
}

//This manager will ensure that the messenger's eventTable will be cleaned up upon loading of a new level.
public sealed class MessengerHelper : MonoBehaviour
{
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
}