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
    [SerializeField] float minAngle = 0f;
    [Tooltip("Angle maximum (ex: 180 ou 90)")]
    [SerializeField] float maxAngle = 180f;

    [Header("Debug / Test")]
    [Tooltip("Coche ça pour tester avec la souris dans l'éditeur")]
    [SerializeField] bool debugWithMouse = false;

    // L'angle actuel du levier
    private float currentAngle;

    private void Start()
    {
        // Initialisation de l'angle basé sur la rotation actuelle
        // On projette la rotation actuelle sur l'axe défini
        Vector3 currentForward = handle.localRotation * Vector3.forward;
        // C'est une approximation pour démarrer, souvent 0
        currentAngle = minAngle;
        ApplyRotation(currentAngle);
    }

    private void Update()
    {
        // Juste pour tester dans l'éditeur Unity sans casque VR
        if (debugWithMouse && Input.GetMouseButton(0))
        {
            // On simule une position de main basée sur la souris (projection sur un plan)
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane p = new Plane(transform.right, transform.position);
            if (p.Raycast(ray, out float enter))
            {
                UpdateLever(ray.GetPoint(enter));
            }
        }
    }

    /// <summary>
    /// À appeler par ton système d'interaction VR (ex: XR Grab Interactable -> OnProcess)
    /// </summary>
    /// <param name="targetPosition">La position mondiale de la main/controlleur</param>
    public void UpdateLever(Vector3 targetPosition)
    {
        // 1. Convertir la position de la cible (main) en espace local par rapport au parent (Base)
        // Cela nous permet d'ignorer la rotation du monde de la base
        Vector3 localTargetPos = transform.InverseTransformPoint(targetPosition);

        // 2. Projeter ce vecteur sur le plan de rotation.
        // Si on tourne autour de X (Right), on s'intéresse aux coordonnées Y et Z.
        // On utilise atan2 pour obtenir l'angle en radians, puis on convertit en degrés.
        // Note: Mathf.Atan2(y, x) -> ici on adapte selon l'axe choisi.

        float targetAngle = 0f;

        // Logique pour l'axe X (le plus commun pour un levier avant/arrière)
        if (rotationAxis == Vector3.right || rotationAxis == -Vector3.right)
        {
            // On calcule l'angle du vecteur (Y, Z) par rapport au "Haut" ou "Avant"
            // Ici on assume que Z est "avant" et Y est "haut".
            targetAngle = Vector3.SignedAngle(Vector3.up, localTargetPos, Vector3.right);

            // Petit ajustement : SignedAngle retourne -180 à 180. 
            // On veut souvent mapper ça différemment selon la modélisation 3D.
            // Si le levier à plat est à 90°, debout à 0°...
            // Pour simplifier, on décale souvent pour que "tout en bas" soit minAngle.
        }
        else
        {
            // Fallback générique (moins précis pour le feeling "pull", mais fonctionnel)
            Vector3 direction = localTargetPos.normalized;
            targetAngle = Vector3.Angle(Vector3.up, direction);
        }

        // 3. Clamper l'angle entre min et max
        // C'est ici qu'on limite la rotation à 180 degrés max défini dans l'inspecteur
        currentAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);

        // 4. Appliquer la rotation
        ApplyRotation(currentAngle);

        // 5. Calculer le ratio (0 à 1) et l'envoyer aux tapis
        float speedRatio = Mathf.InverseLerp(minAngle, maxAngle, currentAngle);

        if (treadmillController != null)
        {
            treadmillController.SetTargetSpeedRatio(speedRatio);
        }
    }

    private void ApplyRotation(float angle)
    {
        // UTILISATION DE QUATERNION demandée
        // Quaternion.AngleAxis crée une rotation de 'angle' degrés autour de 'rotationAxis'
        Quaternion targetRot = Quaternion.AngleAxis(angle, rotationAxis);

        // On applique en local pour rester enfant de la base correctement
        handle.localRotation = targetRot;
    }
}