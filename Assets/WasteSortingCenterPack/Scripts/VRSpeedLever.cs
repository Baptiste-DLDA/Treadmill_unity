using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Nécessaire pour l'interaction
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Nécessaire pour XRBaseInteractable (Unity 6 / XRI 3.0+)
using UnityEngine.XR.Interaction.Toolkit.Interactors;   // Nécessaire pour récupérer la main

// 1. On change l'héritage ici
public class VRSpeedLever : XRBaseInteractable
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
    [Tooltip("Angle de départ")]
    [SerializeField] float moyAngle = 0f;

    [Header("Debug / Contrôle Manuel")]
    [Tooltip("Utilisez ce curseur en mode Play pour tester la vitesse sans le casque VR. Si vous attrapez le levier, ce curseur suivra votre main.")]
    [Range(0f, 1f)]
    [SerializeField] private float debugSpeedRatio = 0.5f;

    private float currentAngle;
    private IXRSelectInteractor interactorHand; // Pour stocker la main qui attrape

    // On utilise Awake au lieu de Start souvent avec XRBaseInteractable, mais Start marche aussi
    protected override void Awake()
    {
        base.Awake(); // Toujours appeler la base
        currentAngle = moyAngle;
        ApplyRotation(currentAngle);
        debugSpeedRatio = Mathf.InverseLerp(minAngle, maxAngle, currentAngle);
    }

    // 2. Détecter quand on attrape le levier
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        interactorHand = args.interactorObject; // On mémorise la main
    }

    // 3. Détecter quand on lâche le levier
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        interactorHand = null; // On oublie la main
    }

    // 4. La boucle de mise à jour spécifique au XR (remplace l'Update classique pour l'interaction)
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        // Si on est dans la phase dynamique (frame), qu'on est sélectionné et qu'on a une main valide
        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            // CAS 1 : Le levier est attrapé par la main VR
            if (isSelected && interactorHand != null)
            {
                UpdateLeverFromHand(interactorHand.transform.position);
            }
            // CAS 2 : Le levier est libre -> on écoute le curseur de l'Inspector
            else
            {
                UpdateLeverFromInspector();
            }
        }
    }

    public void UpdateLeverFromHand(Vector3 targetPosition)
    {
        // 1. Convertir la position de la cible (main) en espace local par rapport au parent (Base)
        // NOTE: Assurez-vous que ce script est sur l'objet "Base" fixe, pas sur le levier qui bouge.
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

        debugSpeedRatio = Mathf.InverseLerp(minAngle, maxAngle, currentAngle);
        // 4. Appliquer la rotation
        ApplyValues();
    }
    private void UpdateLeverFromInspector()
    {
        // On convertit le 0-1 du slider en Angle (-90 à 90 par exemple)
        currentAngle = Mathf.Lerp(minAngle, maxAngle, debugSpeedRatio);

        // On applique
        ApplyValues();
    }
    // Fonction commune pour appliquer la rotation et la vitesse
    private void ApplyValues()
    {
        // Rotation visuelle
        ApplyRotation(currentAngle);

        // Envoi de la vitesse au tapis
        if (treadmillController != null)
        {
            // debugSpeedRatio contient déjà la valeur 0 à 1 correcte dans les deux cas
            treadmillController.SetTargetSpeedRatio(debugSpeedRatio);
        }
    }

    private void ApplyRotation(float angle)
    {
        Quaternion targetRot = Quaternion.AngleAxis(angle, rotationAxis);
        handle.localRotation = targetRot;
    }

}