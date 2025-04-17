using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class FreeCameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float verticalSpeed = 5f;
    public float lookSensitivity = 2f;

    [Header("UI Activation Area")]
    public RectTransform activationArea;
    public GraphicRaycaster raycaster;

    private bool isControlEnabled = false;
    private bool justEnabledControl = false;

    void Start()
    {
        EnableCursor(true);
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // ESC — выйти из режима управления
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EnableCursor(true);
            return;
        }

        // Клик по области без UI — включаем управление
        if (!isControlEnabled && Input.GetMouseButtonDown(0))
        {
            if (ClickedOnThisRectOnly(activationArea))
            {
                EnableCursor(false);
            }
        }

        if (isControlEnabled)
        {
            HandleMouseLook();
            HandleMovement();
        }
    }

    void HandleMovement()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.A)) move -= transform.right;
        if (Input.GetKey(KeyCode.D)) move += transform.right;

        if (Input.GetKey(KeyCode.Q)) move += Vector3.down;
        if (Input.GetKey(KeyCode.E)) move += Vector3.up;

        transform.position += move.normalized * moveSpeed * Time.deltaTime;
    }

    void HandleMouseLook()
    {
        // Пропустить первый кадр после включения управления
        if (justEnabledControl)
        {
            justEnabledControl = false;
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        Vector3 angles = transform.localEulerAngles;
        angles.x -= mouseY;
        angles.y += mouseX;
        angles.z = 0;

        transform.localEulerAngles = angles;
    }

    void EnableCursor(bool enable)
    {
        isControlEnabled = !enable;
        Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enable;

        if (!enable)
            justEnabledControl = true; // Активируем флаг пропуска одного кадра
    }

    // Проверка: был ли клик именно по нашей области, без других UI-элементов сверху
    bool ClickedOnThisRectOnly(RectTransform targetRect)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject == null) continue;

            if (result.gameObject.transform != targetRect)
                return false;
        }

        return RectTransformUtility.RectangleContainsScreenPoint(targetRect, Input.mousePosition);
    }
}
