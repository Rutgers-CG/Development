using UnityEngine;
using System.Collections;

public enum CAMERA_MODE {
    FREE_FLIGHT,
    THIRD_PERSON,
    ISOMETRIC
}

public class CamController : MonoBehaviour {

    public enum CAMERA_CONTROLS {
        FORWARD = KeyCode.W,
        BACKWARD = KeyCode.S,
        LEFT = KeyCode.A,
        RIGHT = KeyCode.D,
        ROTATE_LOCAL_LEFT = KeyCode.Q,
        ROTATE_LOCAL_RIGHT = KeyCode.E,
        SPEED_UP = KeyCode.LeftShift,
        SLOW_DOWN = KeyCode.LeftControl,
        LOOK_AROUND_DRAG = KeyCode.Mouse1
    }

    #region Properties
    [SerializeField]
    public bool EnableController = true;

    [SerializeField]
    public float CameraSpeed;

    [SerializeField]
    public float RotationSpeed;

    [SerializeField]
    public float ZoomSpeed = 10f;

    [SerializeField]
    public CAMERA_MODE CameraMode;
    #endregion

    #region Members
    Vector3 g_TargetOffset;
    Transform g_Target;
    Vector3 g_LastMousePosition;
    bool g_DraggingEvent = false;
    #endregion

    #region Unity_Functions
    // Handle IO
    void FixedUpdate() {
        if(EnableController) {
            switch(CameraMode) {
                case CAMERA_MODE.FREE_FLIGHT:
                    HandleFreeFlight();
                    break;
            }
        }
    }

    void Update() {
        g_LastMousePosition = Input.mousePosition;
    }
    #endregion

    #region Private_Functions
    void HandleFreeFlight() {

        float mod = CameraSpeed;
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetKey((KeyCode)CAMERA_CONTROLS.SPEED_UP)) {
            mod *= 1.2f;    
        } else if (Input.GetKey((KeyCode)CAMERA_CONTROLS.SLOW_DOWN)) {
            mod *= 0.3f;
        }

        if (Input.GetKey((KeyCode) CAMERA_CONTROLS.FORWARD)) {
            transform.position += transform.forward * Time.deltaTime * mod;
        } else if (Input.GetKey((KeyCode)CAMERA_CONTROLS.BACKWARD)) {
            transform.position += (-1 * transform.forward) * Time.deltaTime * mod;
        }

        if(Input.GetKey((KeyCode)CAMERA_CONTROLS.LEFT)) {
            transform.position += (-1 * transform.right) * Time.deltaTime * mod;
        } else if (Input.GetKey((KeyCode)CAMERA_CONTROLS.RIGHT)) {
            transform.position += transform.right * Time.deltaTime * mod;
        }

        if (Input.GetKey((KeyCode)CAMERA_CONTROLS.LOOK_AROUND_DRAG)) {
            var diff = g_LastMousePosition - Input.mousePosition;
            if(diff != Vector3.zero) {
                g_DraggingEvent = true;
                transform.Rotate(transform.right, diff.y * Time.deltaTime * RotationSpeed, Space.World);
                transform.Rotate(Vector3.up, -1 * diff.x * Time.deltaTime * RotationSpeed, Space.World);
            }
        } else {
            g_DraggingEvent = false;
        }

        if (zoomDelta != 0f) {
            Vector3 targetPos = transform.position + Vector3.up * -1 * zoomDelta;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * ZoomSpeed);
        }

    }
    #endregion
}
