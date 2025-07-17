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
    [Header("Ping Settings")]
    [SerializeField] private float _pingIntervalInSeconds = 5f;
    private Coroutine _pingRoutine;
    public double EstimatedServerTime => TimeSyncUtils.GetUnixTimeSeconds() + ClockOffset;
    public double RoundTripTime { get; private set; }
    public double ClockOffset { get; private set; }

    private double _lastPingSendTime;
    private bool _awaitingPong;

    private void Start()
    {
        _pingRoutine = StartCoroutine(PingLoop());
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {

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
        _lastPingSendTime = TimeSyncUtils.GetUnixTimeSeconds();
        _awaitingPong = true;

        var ping = new PingMessage
        {
            Timestamp = _lastPingSendTime
        };

        NetworkManager.Instance.SendMessage(ping);
    }

    public void HandlePong(PongMessage pong)
    {
        if (!_awaitingPong) return;

        double now = TimeSyncUtils.GetUnixTimeSeconds();
        RoundTripTime = now - pong.ClientSendTime;

        double estimatedServerTime = pong.ServerReceiveTime + (RoundTripTime / 2.0);
        ClockOffset = estimatedServerTime - now;

        _awaitingPong = false;

        Debug.Log($"[TimeSync] RTT: {RoundTripTime:F4}s, Offset: {ClockOffset:F4}s, ServerTime: {EstimatedServerTime:F4}");
    }



}