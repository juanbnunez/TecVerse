using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayer : NetworkBehaviour
{
    [Header("Avatar Offsets")]
    [SerializeField] private Vector3 avatarLeftPositionOffset, avatarRightPositionOffset;
    [SerializeField] private Quaternion avatarLeftRotationOffset, avatarRightRotationOffset;
    [SerializeField] private Vector3 avatarHeadPositionOffset;
    [SerializeField] private Vector3 avatarBodyPositionOffset;

    private Transform headTransform, leftControllerTransform, rightControllerTransform;
    
    private Transform avHead, avLeft, avRight, avBody;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        StartCoroutine(SetupReferences());
    }

    private IEnumerator SetupReferences()
    {
        yield return null;

        // Buscar el OVRCameraRig para la cabeza
        OVRCameraRig cameraRig = FindObjectOfType<OVRCameraRig>();
        if (!cameraRig)
        {
            Debug.LogError("No se encontró OVRCameraRig.");
            yield break;
        }
        headTransform = cameraRig.centerEyeAnchor;

        // Buscar controladores dentro de OVRInteractionComprehensive
        GameObject rigRoot = GameObject.Find("OVRInteractionComprehensive");
        if (!rigRoot)
        {
            Debug.LogError("No se encontró OVRInteractionComprehensive.");
            yield break;
        }

        leftControllerTransform = rigRoot.transform.Find("OVRControllers/LeftController");
        rightControllerTransform = rigRoot.transform.Find("OVRControllers/RightController");

        if (leftControllerTransform == null || rightControllerTransform == null)
        {
            Debug.LogError("No se encontraron los controladores.");
            yield break;
        }

        // Referencias a las partes del avatar
        avLeft = transform.Find("Left Hand");
        avRight = transform.Find("Right Hand");
        avHead = transform.Find("Head");
        avBody = transform.Find("Body");
    }

    private void Update()
    {
        if (!IsOwner || headTransform == null) return;

        // Mano izquierda
        if (avLeft && leftControllerTransform)
        {
            avLeft.rotation = leftControllerTransform.rotation * avatarLeftRotationOffset;
            avLeft.position = leftControllerTransform.position +
                              avatarLeftPositionOffset.x * leftControllerTransform.right +
                              avatarLeftPositionOffset.y * leftControllerTransform.up +
                              avatarLeftPositionOffset.z * leftControllerTransform.forward;
        }

        // Mano derecha
        if (avRight && rightControllerTransform)
        {
            avRight.rotation = rightControllerTransform.rotation * avatarRightRotationOffset;
            avRight.position = rightControllerTransform.position +
                               avatarRightPositionOffset.x * rightControllerTransform.right +
                               avatarRightPositionOffset.y * rightControllerTransform.up +
                               avatarRightPositionOffset.z * rightControllerTransform.forward;
        }

        // Cabeza
        if (avHead)
        {
            avHead.rotation = headTransform.rotation;
            avHead.position = headTransform.position +
                              avatarHeadPositionOffset.x * headTransform.right +
                              avatarHeadPositionOffset.y * headTransform.up +
                              avatarHeadPositionOffset.z * headTransform.forward;
        }

        // Cuerpo
        if (avBody && avHead)
        {
            avBody.position = avHead.position + avatarBodyPositionOffset;
        }
    }
}
