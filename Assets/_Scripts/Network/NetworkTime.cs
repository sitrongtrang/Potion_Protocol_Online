using System;
using System.Collections;
using UnityEngine;

public class NetworkTime : MonoBehaviour
{
    public static NetworkTime Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [Header("Constants")]
    public const int AVG_RTT = 150;
    [Header("Ping Settings")]
    private const double SmoothingFactor = 0.1;
    [SerializeField] private float _pingIntervalInSeconds = 5f;
    private Coroutine _pingRoutine;
    public long EstimatedServerTime => TimeSyncUtils.GetUnixTimeMilliseconds() + ClockOffset;
    public long RoundTripTime { get; private set; }
    public long ClockOffset { get; private set; }
    private bool _awaitingPong;

    private void Start()
    {
        
        _pingRoutine = StartCoroutine(PingLoop());
    }

    void OnDestroy()
    {
        if (_pingRoutine != null)
        {
            StopCoroutine(_pingRoutine);
            _pingRoutine = null;
        }
    }

    private IEnumerator PingLoop()
    {
        while (true)
        {
            SendPing();
            yield return new WaitForSeconds(_pingIntervalInSeconds);
        }
    }

    


    private void SendPing()
    {
        _awaitingPong = true;

        var ping = new PingMessage
        {
            ClientSendTime = TimeSyncUtils.GetUnixTimeMilliseconds()
        };

        NetworkManager.Instance.SendMessage(ping);
    }

    public void HandlePong(PongMessage pong)
    {
        if (!_awaitingPong) return;

        long now = TimeSyncUtils.GetUnixTimeMilliseconds();
        RoundTripTime = now - pong.ClientSendTime;

        // if (RoundTripTime > 300) return; // Ignore bad sample

        long estimatedServerTime = (long)(pong.ServerReceiveTime + (RoundTripTime / 2.0));
        ClockOffset = (long)((1 - SmoothingFactor) * ClockOffset + SmoothingFactor * (estimatedServerTime - now));

        _awaitingPong = false;

        Debug.Log($"[TimeSync] RTT: {RoundTripTime:F4}ms, Offset: {ClockOffset:F4}ms, ServerTime: {EstimatedServerTime:F4}ms");
    }

}