using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Script responsável pela movimentação básica 2D do jogador (andar e pular com física).
/// Desenvolvido para a disciplina de Desenvolvimento de Jogos Digitais (Ciência da Computação).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [Tooltip("Velocidade horizontal de deslocamento do jogador")]
    [SerializeField] private float moveSpeed = 7f;

    [Tooltip("Força vertical aplicada ao pular")]
    [SerializeField] private float jumpForce = 12f;

    [Header("Verificação de Chão")]
    [Tooltip("Ponto de origem para checar se o jogador está sobre o chão")]
    [SerializeField] private Transform groundCheck;

    [Tooltip("Raio da esfera/círculo de detecção do chão")]
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Tooltip("Camada (Layer) correspondente ao chão e plataformas")]
    [SerializeField] private LayerMask groundLayer;

    // Componentes cacheados
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // Variáveis de controle de estado e entrada
    private float horizontalInput;
    private bool jumpRequested;
    private bool isGrounded;

    private void Awake()
    {
        // Cacheia as referências necessárias para melhor desempenho
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // 1. Leitura de entrada horizontal (A/D ou Setas)
        horizontalInput = ReadHorizontalInput();

        // 2. Leitura de entrada de pulo (Barra de Espaço)
        // Só solicita o pulo se a tecla foi pressionada neste frame e o jogador está no chão
        if (ReadJumpInput() && isGrounded)
        {
            jumpRequested = true;
        }

        // 3. Orientação visual do sprite (inverter face conforme a direção)
        if (spriteRenderer != null)
        {
            if (horizontalInput > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (horizontalInput < -0.01f)
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    private void FixedUpdate()
    {
        // 1. Checagem se o jogador está em contato com o chão
        CheckGrounded();

        // 2. Movimentação horizontal aplicando velocidade física no Rigidbody2D
        // Mantém a velocidade vertical atual (rb.linearVelocity.y) para não interferir na gravidade
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // 3. Aplicação do pulo se tiver sido solicitado
        if (jumpRequested)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpRequested = false;
        }
    }

    /// <summary>
    /// Verifica se o jogador está encostando no chão utilizando Physics2D.OverlapCircle.
    /// </summary>
    private void CheckGrounded()
    {
        Vector2 checkPosition = groundCheck != null ? (Vector2)groundCheck.position : (Vector2)transform.position;
        isGrounded = Physics2D.OverlapCircle(checkPosition, groundCheckRadius, groundLayer);
    }

    /// <summary>
    /// Lê a entrada horizontal suportando o Novo Sistema de Inputs e o sistema legado como fallback.
    /// </summary>
    private float ReadHorizontalInput()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            float input = 0f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                input += 1f;
            }
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                input -= 1f;
            }
            return input;
        }
#endif
        return Input.GetAxisRaw("Horizontal");
    }

    /// <summary>
    /// Lê a tecla de pulo (Espaço) no frame atual.
    /// </summary>
    private bool ReadJumpInput()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            return Keyboard.current.spaceKey.wasPressedThisFrame;
        }
#endif
        return Input.GetKeyDown(KeyCode.Space);
    }

    /// <summary>
    /// Desenha o gizmo de verificação de chão na Scene View para facilitar o ajuste no editor.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Vector3 center = groundCheck != null ? groundCheck.position : transform.position;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(center, groundCheckRadius);
    }
}
