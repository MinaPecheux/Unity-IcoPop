using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class TargetManager : MonoBehaviour
{
    public static List<TargetManager> MANAGERS;

    [SerializeField] protected GameObject _normalMesh;
    [SerializeField] protected GameObject _fracturedMesh;
    [SerializeField] protected GameObject _vfxExplosion;
    [SerializeField] private AudioClip _onTakeHitSound;
    protected Rigidbody _rigidbody;
    protected Collider _collider;
    protected AudioSource _audioSource;

    protected GameManager _gameManager;

    private bool _isAlive;
    protected bool _canBeHit;

    protected virtual void Awake()
    {
        // auto-register instance
        if (MANAGERS == null)
            MANAGERS = new List<TargetManager>();
        MANAGERS.Add(this);

        _gameManager = GameManager.instance;

        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _audioSource = GetComponent<AudioSource>();
        GetComponent<XRSimpleInteractable>().interactionManager =
            _gameManager.interactionManager;

        _isAlive = true;
        _canBeHit = true;
    }

    private void Start()
    {
        _rigidbody.AddForce(
            -transform.position.normalized * _gameManager.speed,
            ForceMode.VelocityChange);
    }

    void Update()
    {
        if (_isAlive && transform.position.sqrMagnitude < 1f)
        {
            _gameManager.TakeHit();
            Destroy();
        }
    }

    #region Public Methods
    public virtual void TakeHit()
    {
        _audioSource.PlayOneShot(_onTakeHitSound);
        _canBeHit = false;
        Invoke("_ResetCanBeHit", 0.2f);
    }

    public void Destroy()
    {
        Destroy(gameObject);
        MANAGERS.Remove(this);
    }
    #endregion

    #region Protected/Private Methods
    protected void _Die()
    {
        _gameManager.IncreaseScore();
        _collider.enabled = false;
        _vfxExplosion.SetActive(true);
        _fracturedMesh.SetActive(true);
        _normalMesh.SetActive(false);
        _isAlive = false;
        StartCoroutine(_Dying());
    }

    protected IEnumerator _Dying()
    {
        float t = 0f;
        while (t < 0.9f)
        {
            _audioSource.volume = 1 - t;
            t += Time.deltaTime;
            yield return null;
        }
        Destroy();
    }

    private void _ResetCanBeHit()
    {
        _canBeHit = true;
    }
    #endregion
}
