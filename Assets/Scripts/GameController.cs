using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    public float sensitivity;
    public float rotationSpeed;
    public GameObject[] gameObjectsOnScene;

    [SerializeField]
    private Color[] _cubesColors;

    private int _currentCubeId;

    private float rotationY;
    private float rotationX;
    private bool isFocused;

    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Camera _camera;

    private GameObject _currentFocusedObject;


    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        isFocused = false;

        _cubesColors = new Color[gameObjectsOnScene.Length];
        for (int i = 0; i < _cubesColors.Length; i++)
        {
            MeshRenderer meshRenderer = gameObjectsOnScene[i].GetComponent<MeshRenderer>();
            Material material = meshRenderer.sharedMaterial;
            _cubesColors[i] = material.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        { 
            if (Input.GetMouseButton(0))
            {
                if (isFocused)
                    CameraAroundObject(_currentFocusedObject);
                if (!isFocused)
                    MoveCamera();
            }

            if (Input.GetMouseButtonDown(1))
            {
                SetCameraPosition();
            }
        }   
    }

    private void MoveCamera()
    {
        rotationY += Input.GetAxis("Mouse Y") * -sensitivity;
        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        transform.localEulerAngles = new Vector3(rotationY, rotationX, 0);
    }

    private void CameraAroundObject(GameObject rotationObject)
    {
        if (Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0)
        {
            float verticalInput = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            float horizontalInput = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            Vector3 rotationAxis = new Vector3(0, horizontalInput, verticalInput);
            transform.transform.RotateAround(rotationObject.transform.position, rotationAxis, rotationSpeed * Time.deltaTime);
        }
    }

    private void SetCameraPosition()
    {
        Ray hitRay = new Ray(transform.position, transform.forward);

        if (!Physics.Raycast(hitRay, out var hit)) return;


        if (!isFocused)
        {
            if (hit.collider.CompareTag("ObjectToMoveAround"))
            {
                FocusOnObject(hit);
            }
        }
        else
        {
            ResetCameraPosition();
        }

    }

    private void FocusOnObject(RaycastHit objectToFocus)
    {
        isFocused = true;
        _currentFocusedObject = objectToFocus.transform.gameObject;
        transform.position = new Vector3(_currentFocusedObject.transform.position.x - 2f, 0, _currentFocusedObject.transform.position.z);
        transform.LookAt(_currentFocusedObject.transform.position);
    }    

    private void ResetCameraPosition()
    {
        transform.position = _startPosition;
        transform.rotation = _startRotation;
        isFocused = false;
    }

    private void ChangeObjectAlpha(Material objectMaterial, float alpha)
    {
    }

    public void ChangeAlpha(float alpha)
    {
        MeshRenderer cubeRenderer = gameObjectsOnScene[_currentCubeId].GetComponent<MeshRenderer>();
        Material cubeMaterial = cubeRenderer.sharedMaterial;


        Color newColor = cubeMaterial.color;
        newColor.a = alpha;

        cubeMaterial.SetColor("_Color", newColor);
    }

    public void SetCubeId(int cubeId)
    {
        _currentCubeId = cubeId;
    }

    public void ToggleCubeVisibility(int cubeId)
    {
        gameObjectsOnScene[cubeId].SetActive(!gameObjectsOnScene[cubeId].activeInHierarchy);
    }

    public void ChangeColor(int cubeId)
    {
        Material cubeMaterial = gameObjectsOnScene[cubeId].GetComponent<MeshRenderer>().sharedMaterial;
        if (cubeMaterial.color == Color.white)
        {
            cubeMaterial.color = _cubesColors[cubeId];
        } else
        {
            cubeMaterial.color = Color.white;
        }
    }
}
