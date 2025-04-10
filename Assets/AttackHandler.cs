using UnityEngine;
using TMPro;
using UnityEngine.UI; 
public class AttackHandler : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] Rigidbody2D _enemyRb;
    [SerializeField] FixedJoint2D _enemyJoint;
    [SerializeField] float _maxForce = 20f;
    [SerializeField] float _enemyWeight = 7f;
    [SerializeField] Vector2 _attackDirection = Vector2.right;

    [Header("Timing Settings")]
    [SerializeField] int _totalSteps = 3;
    [SerializeField] float _timingWindow = 0.5f;
    private int _currentStep = 0;
    private float _accumulatedForce = 0f;
    private float _redTime;
    private bool _canPress = false;

    [Header("Marker Visuals")]
    [SerializeField] SpriteRenderer[] _markerSprites;
    [SerializeField] float _floatSpeed = 1f;
    [SerializeField] float _radius = 2f;
    [SerializeField] Color _idleColor = Color.white;
    [SerializeField] Color _activeColor = Color.red;
    [SerializeField] float _colorChangeSpeed = 2f;

    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI _forceText;
    [SerializeField] TextMeshProUGUI _weightText;
    [SerializeField] TextMeshProUGUI _resultText;
    [SerializeField] GameObject _retryPanel; 
    [SerializeField] Button _retryButton;    

    private Transform _enemyTransform;

    void Start()
    {
        _enemyTransform = _enemyRb.transform;
        UpdateUI();
        StartAttackSequence();

        _retryButton.onClick.AddListener(RestartAttackSequence);
        _retryPanel.SetActive(false); 
    }

    void Update()
    {
        UpdateMarkerPositions();

        if (_canPress && Input.GetKeyDown(KeyCode.Space))
        {
            float timeOffset = Mathf.Abs(Time.time - _redTime);
            float forceFromHit = Mathf.Clamp01(1f - (timeOffset / _timingWindow));
            _accumulatedForce += forceFromHit * (_maxForce / _totalSteps) * 1.5f;

            _canPress = false;
            UpdateUI();

            _currentStep++;

            if (_currentStep < _totalSteps)
                Invoke(nameof(NextMarker), 1f);
            else
                Invoke(nameof(ApplyFinalForce), 0.5f);
        }

        if (_canPress)
        {
            float lerpTime = Mathf.Abs(Time.time - _redTime) / _timingWindow;
            Color lerpedColor = Color.Lerp(_idleColor, _activeColor, Mathf.PingPong(Time.time * _colorChangeSpeed, 1));
            _markerSprites[_currentStep].color = lerpedColor;
        }
    }

    void StartAttackSequence()
    {
        _accumulatedForce = 0;
        _currentStep = 0;
        UpdateUI();
        NextMarker();
    }

    void NextMarker()
    {
        _redTime = Time.time;
        _canPress = true;

        for (int i = 0; i < _markerSprites.Length; i++)
        {
            if (i == _currentStep)
                _markerSprites[i].color = _idleColor;
            else
                 _markerSprites[i].color = _idleColor;
        }
    }

    void ApplyFinalForce()
    {
        
        _canPress = false;

        foreach (var sprite in _markerSprites)
            sprite.color = _idleColor;

        if (_accumulatedForce >= _enemyWeight)
        {
            if (_enemyJoint != null)
                Destroy(_enemyJoint);
            DisplayResultText("Success! Enemy is knocked back.");
        }
        else
        {
            DisplayResultText("Too weak! Try again.");
        }

        _enemyRb.AddForce(_attackDirection.normalized * _accumulatedForce, ForceMode2D.Impulse);

        Invoke(nameof(ShowRetryPanel), 2f);
    }

    void UpdateUI()
    {
        if (_forceText != null)
            _forceText.text = "Attack Power: " + _accumulatedForce.ToString("F1") + " / " + _maxForce;

        if (_weightText != null)
            _weightText.text = "Enemy Weight: " + _enemyWeight.ToString("F1");
    }

    void UpdateMarkerPositions()
    {
        float angle = Time.time * _floatSpeed;

        for (int i = 0; i < _markerSprites.Length; i++)
        {
            float x = Mathf.Cos(angle + (i * Mathf.PI * 2f / _markerSprites.Length)) * _radius;
            float y = Mathf.Sin(angle + (i * Mathf.PI * 2f / _markerSprites.Length)) * _radius;
            _markerSprites[i].transform.position = _enemyTransform.position + new Vector3(x, y, 0f);
        }
    }

    void DisplayResultText(string result)
    {
        if (_resultText != null)
        {
            _resultText.text = result;
            _resultText.gameObject.SetActive(true);
            Invoke(nameof(HideResultText), 2f);
        }
    }

    void HideResultText()
    {
        if (_resultText != null)
        {
            _resultText.gameObject.SetActive(false);
        }
    }

    void ShowRetryPanel()
    {
        _retryPanel.SetActive(true);
    }

    void RestartAttackSequence()
    {
        _retryPanel.SetActive(false); 
        StartAttackSequence();       
    }
}
