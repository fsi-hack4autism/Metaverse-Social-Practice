using UnityEngine;
using UnityEngine.Events;

public class ClickHandler : MonoBehaviour
{
    public int[] ignoreLayers = new int[] { 2 };
    public float hoverDistance = -1;
    [Tooltip("After being triggered, waits this long before being able to trigger again.")]
    public float Cooldown = 0f;
    private float _cooldownTimer = 0f;

    public UnityEvent OnClicked = new UnityEvent();
    public UnityEvent OnPressStart = new UnityEvent();
    public UnityEvent OnPressEnd = new UnityEvent();
    public UnityEvent OnHoverStart = new UnityEvent();
    public UnityEvent OnHoverEnd = new UnityEvent();

    // ==========	STATUS	==========
    private bool isHovering = false;
    private bool isPressing = false;
    public Vector3 mousePos { get; private set; }
    public Vector2 mousePos2D { get; private set; }
    public Ray mouseRay { get; private set; }
    public RaycastHit mouseHit { get; private set; }




    private void OnEnable()
    {
        _cooldownTimer = Time.time - Cooldown;
    }

    private void Update()
    {
        // handle cooldown
        if (Time.time - _cooldownTimer < Cooldown) return;

        // Ignore if clicking on canvas
        var eventSys = UnityEngine.EventSystems.EventSystem.current;
        if (eventSys && eventSys.IsPointerOverGameObject())
            return;

        // check for hover
        if (!Camera.main) return;
        mousePos2D = Input.mousePosition;
        mouseRay = Camera.main.ScreenPointToRay(mousePos2D);
        RaycastHit hit;
        float distance = hoverDistance < 0 ? Mathf.Infinity : hoverDistance;
        // ignore layers in ignoreLayers
        int layerMask = -1;
        for (int i = 0; i < ignoreLayers.Length; i++)
        {
            if (i == 0)
                layerMask = 1 << ignoreLayers[i];
            else
                layerMask = layerMask | 1 << ignoreLayers[i];
        }
        layerMask = ~layerMask;
        // raycast
        if (Physics.Raycast(mouseRay, out hit, distance, layerMask))
        {
            mouseHit = hit;
            // if raycast hits this object
            if (GetComponent<Rigidbody>()) // rigidbody check
            {
                if (mouseHit.collider.attachedRigidbody && mouseHit.collider.attachedRigidbody.gameObject.Equals(this.gameObject))
                {
                    HandleRaycastHover(true);
                    mousePos = mouseRay.origin + mouseRay.direction.normalized * mouseHit.distance;
                }
                else
                {
                    HandleRaycastHover(false);
                    mousePos = Vector3.zero;
                }
            }
            else // collider check
            {
                if (mouseHit.collider.gameObject.Equals(this.gameObject))
                {
                    HandleRaycastHover(true);
                    mousePos = mouseRay.origin + mouseRay.direction.normalized * mouseHit.distance;
                }
                else
                {
                    HandleRaycastHover(false);
                    mousePos = Vector3.zero;
                }
            }
        }
        else
        {
            mousePos = Vector3.zero;
            mouseHit = new RaycastHit();
        }

        // listen for press on object
        if (this.isHovering && Input.GetMouseButtonDown(0))
        {
            OnPressStart.Invoke();
            this.isPressing = true;
        }
        // listen for release on object
        if (this.isPressing && Input.GetMouseButtonUp(0))
        {
            OnPressEnd.Invoke();
            this.isPressing = false;
            OnClicked.Invoke();
            _cooldownTimer = Time.time;
        }
        // listen for release *off* object
        if (!this.isHovering && this.isPressing && Input.GetMouseButtonUp(0))
        {
            OnPressEnd.Invoke();
            this.isPressing = false;
        }
    }

    private void HandleRaycastHover(bool isHovering)
    {
        if (isHovering && !this.isHovering)
            OnHoverStart.Invoke();
        else if (!isHovering && this.isHovering)
            OnHoverEnd.Invoke();

        this.isHovering = isHovering;
    }
}
