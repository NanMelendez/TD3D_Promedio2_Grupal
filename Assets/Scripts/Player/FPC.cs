using UnityEngine;
using UnityEngine.InputSystem;

public class FPC : MonoBehaviour
{
    public float velocidad = 5f;
    public float gravedad = -9.81f;
    public float sensibilidad = 0.5f;

    public Transform camara;
    public InputActionReference mover;
    public InputActionReference mirar;

    private CharacterController controlador;
    private float rotacionX = 0f;
    private float velocidadY = 0f;

    void Start()
    {
        controlador = GetComponent<CharacterController>();

        if (controlador == null)
        {
            controlador = gameObject.AddComponent<CharacterController>();
            controlador.height = 2f;
            controlador.radius = 0.5f;
        }

        if (camara == null)
        {
            camara = Camera.main.transform;
        }
        if (camara.parent != transform)
        {
            camara.SetParent(transform);
            camara.localPosition = new Vector3(0f, 0.6f, 0f);
        }

        if (mover == null || mirar == null)
        {
            Debug.LogError("Pon las acciones Mover y Mirar en el inspector");
            enabled = false;
            return;
        }

        mover.action.Enable();
        mirar.action.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 mouse = mirar.action.ReadValue<Vector2>();

        rotacionX = rotacionX - mouse.y * sensibilidad;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);
        camara.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);

        transform.Rotate(Vector3.up * mouse.x * sensibilidad);

     
        Vector2 input = mover.action.ReadValue<Vector2>();

      
        Vector3 movimiento = transform.right * input.x + transform.forward * input.y;
        controlador.Move(movimiento * velocidad * Time.deltaTime);

        if (controlador.isGrounded && velocidadY < 0f)
        {
            velocidadY = -2f;
        }

        velocidadY = velocidadY + gravedad * Time.deltaTime;
        controlador.Move(new Vector3(0f, velocidadY, 0f) * Time.deltaTime);
    }
}