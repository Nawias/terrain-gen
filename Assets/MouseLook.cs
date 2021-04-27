/**
    Michał Wójcik 2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MouseLook : MonoBehaviour
{
    //Mnożnik czułości myszy
    [SerializeField]
    private float mouseSensitivity = 100f;
    //Mnożnik prędkości poruszania się
    [SerializeField]
    private float movementSpeed = 10f;
    //Zmienna pomocnicza do śledzenia obrotu w osi X (patrzenie w górę i w dół)
    private float xRotation = 0f;

    void Start()
    {
        //Zablokowanie kursora do swobodnego poruszania kamerą
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        UpdateRotation();
        UpdatePosition();
    }

    private void UpdateRotation()
    {
        //Pobranie pozycji myszy w osiach X i Y
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //Aktualizacja rotacji w osi X
        xRotation -= mouseY;
        //Ograniczenie rotacji do 180 stopni
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Rotacja w osi X (góra-dół)
        transform.localRotation = Quaternion.Euler(xRotation, transform.localEulerAngles.y, 0f);
        
        //Rotacja w osi Y (na boki)
        transform.Rotate(Vector3.up, mouseX);
    }
    private void UpdatePosition()
    {
        //Składowe wektora ruchu
        float x = 0f;
        float z = 0f;
        float y = 0f;

        //Pobieranie wartości z klawiatury
        //Ruch w danej osi = kierunek (-1,1) * prędkość ruchu * czas od ostatniej klatki
        if (Input.GetKey(KeyCode.W))
        {
            z += movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            z -= movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            x -= movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            x += movementSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            y += movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            y -= movementSpeed * Time.deltaTime;
        }

        //Utworzenie wektora ruchu ze składowych
        Vector3 move = transform.right * x + transform.forward * z + transform.up * y;
        //Wykonanie ruchu przy pomocy komponentu CharacterController - relatywnie do aktualnej rotacji
        GetComponent<CharacterController>().Move(move);
    }

}
