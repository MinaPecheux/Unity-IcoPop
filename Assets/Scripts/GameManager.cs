using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private const float _INVULNERABILITY_DELAY = 1f;

    [Header("Core")]
    public XRInteractionManager interactionManager;
    [SerializeField] private GameObject[] _targetPrefabs;
    [SerializeField] private AudioSource _audioSourceScore;
    [SerializeField] private AudioSource _audioSourceHit;

    [Header("UI")]
    [SerializeField] private Text _scoreText;
    [SerializeField] private Image[] _healthCellImages;
    [SerializeField] private Sprite _healthCellFilled;
    [SerializeField] private Sprite _healthCellEmpty;
    [SerializeField] private Image _invulnerabilityScreen;
    [SerializeField] private GameObject _startPanel;
    [SerializeField] private GameObject _howToPlayPanel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _gameOverPanel;

    [HideInInspector] public float speed;
    [HideInInspector] public bool inUI;

    private int _lives;
    private int _score;
    private bool _invulnerable;
    private Coroutine _spawningTargetsCoroutine;

    private float _minSpawnTime;
    private float _maxSpawnTime;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        inUI = true;

        _startPanel.SetActive(true);
        _howToPlayPanel.SetActive(false);
        _pausePanel.SetActive(false);
        _gameOverPanel.SetActive(false);
#if UNITY_EDITOR
        Play();
#endif
    }

    #region Public Methods
    public void TakeHit()
    {
        if (_lives < 0 || _invulnerable) return;

        _audioSourceHit.Play();
        StartCoroutine(_ProcessingInvulnerability());
        _lives--;
        if (_lives >= 0)
            _healthCellImages[_lives].sprite = _healthCellEmpty;
        if (_lives <= 0)
            Invoke("_GameOver", 0.5f);
    }

    public void IncreaseScore()
    {
        _score++;
        _scoreText.text = _score.ToString();
        _audioSourceScore.Play();

        if (Random.Range(0f, 1f) < 0.1f)
            _SpawnTarget();
    }

    public void Play()
    {
        _Reset();
        _startPanel.SetActive(false);
        inUI = false;
    }

    public void Pause()
    {
        Time.timeScale = 0;
        _pausePanel.SetActive(true);
        inUI = true;
    }

    public void Unpause()
    {
        Time.timeScale = 1;
        _pausePanel.SetActive(false);
        inUI = false;
    }

    public void ShowHowToPlay()
    {
        _howToPlayPanel.SetActive(true);
    }

    public void HideHowToPlay()
    {
        _howToPlayPanel.SetActive(false);
    }

    public void Replay()
    {
        _Reset();
        _gameOverPanel.SetActive(false);
        inUI = false;
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
#endregion

#region Private Methods
    private void _Reset()
    {
        speed = 3f;

        _lives = 3;
        _score = 0;
        _invulnerable = false;

        _minSpawnTime = 3f;
        _maxSpawnTime = 3.5f;

        foreach (Image i in _healthCellImages)
            i.sprite = _healthCellFilled;

        _spawningTargetsCoroutine = StartCoroutine(_SpawningTargets());
    }

    private IEnumerator _SpawningTargets()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(_minSpawnTime, _maxSpawnTime));
            if (_minSpawnTime > 1f)
            {
                _minSpawnTime -= 0.02f;
                _maxSpawnTime -= 0.02f;
                speed += 0.02f;
            }

            int nTargets = Random.Range(1, 4);
            for (int i = 0; i < nTargets; i++)
                _SpawnTarget();
        }
    }

    private void _SpawnTarget()
    {
        int prefabId = Random.Range(0, _targetPrefabs.Length);
        Vector3 position = _SphericalToCartesian(new Vector3(
            18f,
            Random.Range(0f, Mathf.PI * 2f),
            Random.Range(Mathf.PI / 6f, Mathf.PI / 3f)));
        Instantiate(_targetPrefabs[prefabId], position, Quaternion.identity);
    }

    private Vector3 _SphericalToCartesian(Vector3 spherical)
    {
        // x: radius, y: theta, z: phi
        return spherical.x * new Vector3(
            Mathf.Sin(spherical.z) * Mathf.Cos(spherical.y),
            Mathf.Cos(spherical.z),
            Mathf.Sin(spherical.z) * Mathf.Sin(spherical.y));
    }

    private IEnumerator _ProcessingInvulnerability()
    {
        Color start = new Color(1f, 0f, 0f, 0.5f);
        Color end = new Color(1f, 0f, 0f, 0f);

        _invulnerable = true;
        _invulnerabilityScreen.color = start;

        float t = 0f;
        while (t < _INVULNERABILITY_DELAY)
        {
            _invulnerabilityScreen.color = Color.Lerp(
                start, end, t / _INVULNERABILITY_DELAY);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        _invulnerabilityScreen.color = end;
        _invulnerable = false;
    }

    private void _GameOver()
    {
        if (_spawningTargetsCoroutine != null)
        {
            StopCoroutine(_spawningTargetsCoroutine);
            _spawningTargetsCoroutine = null;
        }

        List<TargetManager> targetManagers =
            new List<TargetManager>(TargetManager.MANAGERS);
        foreach (TargetManager tm in targetManagers)
            tm.Destroy();

        _gameOverPanel.SetActive(true);
        inUI = true;
    }
#endregion

}
