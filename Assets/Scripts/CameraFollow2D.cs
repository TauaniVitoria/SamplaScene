using UnityEngine;

/// <summary>
/// Script simples para fazer a câmera acompanhar suavemente o personagem no plano 2D.
/// </summary>
public class CameraFollow2D : MonoBehaviour
{
    [Tooltip("Alvo a ser seguido pela câmera (o jogador)")]
    [SerializeField] private Transform target;

    [Tooltip("Suavidade da interpolação de movimento da câmera")]
    [SerializeField] private float smoothSpeed = 5f;

    [Tooltip("Deslocamento relativo da câmera em relação ao jogador")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, -10f);

    private void LateUpdate()
    {
        if (target == null) return;

        // Posição desejada mantendo o Z da câmera
        Vector3 targetPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, offset.z);

        // Interpolação suave (Lerp)
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
