using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerManager : MonoBehaviour
{

    [SerializeField] private XRRayInteractor _leftInteractor;
    [SerializeField] private XRRayInteractor _rightInteractor;
    [SerializeField] private AudioSource _audioSource;
    private GameManager _gameManager;

    private RaycastHit _hit;

    private void Awake()
    {
        _gameManager = GetComponent<GameManager>();
    }

    private void Update()
    {
        OVRInput.Update();

        if (OVRInput.GetDown(OVRInput.Button.One))
            _gameManager.Pause();

        if (!_gameManager.inUI)
        {
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5f)
                if (_leftInteractor.TryGetCurrent3DRaycastHit(out _hit))
                    _Fire();

            if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f)
                if (_rightInteractor.TryGetCurrent3DRaycastHit(out _hit))
                    _Fire();
        }
    }

    private void _Fire()
    {
        _audioSource.Play();
        _hit.transform.GetComponent<TargetManager>().TakeHit();
    }

}
