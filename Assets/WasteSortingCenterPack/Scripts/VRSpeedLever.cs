using UnityEngine;
using UnityEngine.Events;

public class VRSpeedLever : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("L'objet qui pivote (le manche du levier)")]
    [SerializeField] Transform handle;
    [Tooltip("Le script qui gère les tapis roulants")]
    [SerializeField] TreadmillsController treadmillController;

    [Header("Paramètres du Levier")]
    [Tooltip("L'axe de rotation local (ex: X pour un mouvement avant/arrière)")]
    [SerializeField] Vector3 rotationAxis = Vector3.right;
    [Tooltip("Angle minimum (ex: 0 ou -90)")]
    [SerializeField] float minAngle = -90f;
    [Tooltip("Angle maximum (ex: 180 ou 90)")]
    [SerializeField] float maxAngle = 90f;

    private float currentAngle;

    private void Start()
    {

        currentAngle = minAngle;
        ApplyRotation(currentAngle);
    }


    public void UpdateLever(Vector3 targetPosition)
    {
        // 1. Convertir la position de la cible (main) en espace local par rapport au parent (Base)

        Vector3 localTargetPos = transform.InverseTransformPoint(targetPosition);

        // 2. Projeter ce vecteur sur le plan de rotation.
        float targetAngle = 0f;

        if (rotationAxis == Vector3.right || rotationAxis == -Vector3.right)
        {

            targetAngle = Vector3.SignedAngle(Vector3.up, localTargetPos, Vector3.right);
        }
        else
        {
            Vector3 direction = localTargetPos.normalized;
            targetAngle = Vector3.Angle(Vector3.up, direction);
        }

        // 3. Clamper l'angle entre min et max
        currentAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);

        // 4. Appliquer la rotation
        ApplyRotation(currentAngle);

        // 5. Calculer le ratio (0 à 1) et l'envoyer aux tapis
        float speedRatio = Mathf.InverseLerp(minAngle, maxAngle, currentAngle);

        if (treadmillController != null)
        {

            treadmillController.SetSpeed(speedRatio);
        }
    }

    private void ApplyRotation(float angle)
    {

        Quaternion targetRot = Quaternion.AngleAxis(angle, rotationAxis);
        handle.localRotation = targetRot;
    }
}