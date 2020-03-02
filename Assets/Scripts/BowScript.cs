using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BowScript : MonoBehaviour
{
    [Header("Bow")]
    public Transform bowModel;
    private Vector3 bowOriginalPos, bowOriginalRot;
    public Transform bowZoomTransform;

    [Space]

    [Header("Arrow")]
    public GameObject arrowPrefab;
    public Transform arrowSpawnOrigin;
    public Transform arrowModel;
    private Vector3 arrowOriginalPos;

    [Space]

    [Header("Parameters")]
    public Vector3 arrowImpulse;
    public float timeToShoot;
    public float shootWait;
    public bool canShoot;
    public bool shootRest = false;
    public bool isAiming = false;

    [Space]

    public float zoomInDuration;
    public float zoomOutDuration;

    private float camOriginalFov;
    public float camZoomFov;
    private Vector3 camOriginalPos;
    public Vector3 camZoomOffset;

    //Add public particle variables here

    private void Start()
    {
        camOriginalPos = Camera.main.transform.localPosition;
        camOriginalFov = Camera.main.fieldOfView;
        bowOriginalPos = bowModel.transform.localPosition;
        bowOriginalRot = bowModel.transform.localEulerAngles;
        arrowOriginalPos = arrowModel.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            if (!shootRest && !isAiming)
            {
                canShoot = false;
                isAiming = true;

                StopCoroutine(PrepareSequence());
                StartCoroutine(PrepareSequence());

                Transform bow = bowZoomTransform;
                arrowModel.transform.localPosition = arrowOriginalPos;
                arrowModel.DOLocalMoveZ(arrowModel.transform.localPosition.z - .10f, zoomInDuration * 2f);
                CameraZoom(camZoomFov, camOriginalPos + camZoomOffset, bow.localPosition, bow.localEulerAngles, zoomInDuration, true);

            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (!shootRest && isAiming)
            {
                StopCoroutine(ShootSequence());
                StartCoroutine(ShootSequence());
            }
        }

    }

    public void CameraZoom(float fov, Vector3 camPos, Vector3 bowPos, Vector3 bowRot, float duration, bool zoom)
    {
        Camera.main.transform.DOComplete();
        Camera.main.DOFieldOfView(fov, duration);
        Camera.main.transform.DOLocalMove(camPos, duration);
        bowModel.transform.DOLocalRotate(bowRot, duration).SetEase(Ease.OutBack);
        bowModel.transform.DOLocalMove(bowPos, duration).OnComplete(() => ShowArrow(zoom));
    }

    public void ShowArrow(bool state)
    {
        bowModel.GetChild(0).gameObject.SetActive(state);
    }

    public IEnumerator PrepareSequence()
    {
        //Play each particle in prepareParticles[]

        yield return new WaitForSeconds(timeToShoot);

        canShoot = true;

        //Play each particle in aimParticles[]
    }

    public IEnumerator ShootSequence()
    {

        yield return new WaitUntil(() => canShoot == true);

        shootRest = true;

        isAiming = false;
        canShoot = false;

        CameraZoom(camOriginalFov, camOriginalPos, bowOriginalPos, bowOriginalRot, zoomOutDuration, true);
        arrowModel.transform.localPosition = arrowOriginalPos;

        //Instantiate circle particle here

        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnOrigin.position, bowModel.rotation);
        arrow.GetComponent<Rigidbody>().AddForce(transform.forward * arrowImpulse.z + transform.up * arrowImpulse.y, ForceMode.Impulse);
        ShowArrow(false);

        yield return new WaitForSeconds(shootWait);
        shootRest = false;
    }

}
