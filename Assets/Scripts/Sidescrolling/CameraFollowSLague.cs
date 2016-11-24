using UnityEngine;
using System.Collections;

public class CameraFollowSLague : MonoBehaviour {

    [SerializeField]
    private Player target;

    [SerializeField]
    private Vector2 focusAreaSize;

    [SerializeField]
    private float verticalOffset;

    [SerializeField]
    private float lookAheadDistanceX;

    [SerializeField]
    private float lookSmoothTimeX;

    [SerializeField]
    private float verticalSmoothTime;

    FocusArea focusArea;

    private float currentLookAheadX;
    private float targetLookAheadX;
    private float lookAheadDirectionX;
    private float smoothLookVelocityX;
    private float smoothVelocityY;

    private bool lookAheadStopped;

    //Maintain reference to original boxcollider size in order to prevent
    //janky camera jerks, eg. player goes to jump, smaller collider is created
    //once original collider is reset, camera adjust for the slightly larger collider
    private Bounds playerBoxColliderBounds;

    // Use this for initialization
    void Start()
    {
        playerBoxColliderBounds = target.GetComponent<BoxCollider2D>().bounds;

        focusArea = new FocusArea(target.GetComponent<BoxCollider2D>().bounds, focusAreaSize);
    }

    void LateUpdate()
    {
        //focusArea.Update(target.GetComponent<BoxCollider2D>().bounds);
        //Center never really changes, this will remove jank cam
        playerBoxColliderBounds.center = target.GetComponent<BoxCollider2D>().bounds.center;

        focusArea.Update(playerBoxColliderBounds);  
                  
        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

        if (focusArea.velocity.x != 0)
        {
            lookAheadDirectionX = Mathf.Sign(focusArea.velocity.x);

            //controls
            //Sign of 0 is 1, so if not moving could be equal
            if ((Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x)) && (target.playerInput.x != 0))
            {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirectionX * lookAheadDistanceX;
            } else
            {
                if (!lookAheadStopped)
                {
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirectionX * lookAheadDistanceX - currentLookAheadX) / 4f;
                }
            }
        }
                
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);

        focusPosition += Vector2.right * currentLookAheadX;

        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }

    void Update()
    {
        if (target == null)
        {
            target = Object.FindObjectOfType<Player>();
        }
    }

    /*void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }*/

    struct FocusArea
    {
        public Vector2 center;
        public Vector2 velocity;
        float left, right;
        float top, bottom;


        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update (Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            } else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }

}
