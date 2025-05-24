using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeController : MonoBehaviour
{
    public Transform leftHandHold; // Assign the Left Rope Handle Transform
    public Transform rightHandHold; // Assign the Right Rope Handle Transform
    public Transform characterTransform; // Assign the main character transform (for relative positioning)

    public int segments = 20; // Number of segments for the rope's curve
    public float ropeSag = 1f; // How much the rope sags downwards from the hands to the swing center
    public float swingSpeed = 10f; // How fast the rope swings (radians per second)
    public float swingRadius = 10f; // Radius of the rope's lowest point circular path

    private LineRenderer lineRenderer;
    private float currentAngle = 0f; // This will go from 0 to 2*PI

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments;
        // Optional: Initialize currentAngle to a specific starting phase if needed
        // For example, if 0 should be "rope behind head ready to swing forward"
        // currentAngle = Mathf.PI; // Start with rope effectively at the top of its arc
    }

    void Update()
    {
        if (leftHandHold == null || rightHandHold == null || characterTransform == null)
        {
            Debug.LogWarning("Hand holds or character transform not assigned!");
            return;
        }

        // Animate the swing angle (if not driven by Animation Events)
        currentAngle += Time.deltaTime * swingSpeed;
        if (currentAngle >= 2 * Mathf.PI)
        {
            currentAngle -= 2 * Mathf.PI; // Loop the angle
        }

        Vector3 ropeStart = leftHandHold.position;
        Vector3 ropeEnd = rightHandHold.position;
        Vector3 midPointHands = (ropeStart + ropeEnd) / 2f;

        // The axis around which the rope's lowest point will swing (character's right/left axis)
        Vector3 rotationAxis = characterTransform.right;

        // The center of the circular path that the rope's lowest point follows.
        // This center is below the hands by 'ropeSag'.
        Vector3 swingPathCenter = midPointHands - characterTransform.up * ropeSag;

        // Calculate the position of the rope's "lowest point" in its swing arc.
        // We want currentAngle = 0 to be "rope under feet" for the formula,
        // so we might need to adjust how currentAngle is used or interpreted if
        // the animation sync implies a different zero point.

        // Let's define an 'effectiveAngle' for the lowest point calculation.
        // If currentAngle = 0 is "top of swing (over head/back)", then effectiveAngle for calculation
        // needs to result in an upward offset.
        // The vector (-characterTransform.up * swingRadius) points DOWN.
        // Rotating this DOWN vector by 'currentAngle' around 'rotationAxis' (character.right):
        // - If currentAngle = 0, lowestPoint is DOWN.
        // - If currentAngle = PI/2, lowestPoint is FORWARD or BACKWARD (depending on right-hand rule and axis sign).
        // - If currentAngle = PI, lowestPoint is UP.
        // - If currentAngle = 3PI/2, lowestPoint is BACKWARD or FORWARD.

        // To match the video (start over head, then front, then under, then back):
        // - Over head: effective angle should be PI
        // - Down in front: effective angle should be 3PI/2 (or -PI/2)
        // - Under feet: effective angle should be 0 (or 2PI)
        // - Up the back: effective angle should be PI/2

        // Let's make the currentAngle that increments via swingSpeed directly map to this:
        // currentAngle = 0 means Over Head
        // currentAngle = PI/2 means Down in Front
        // currentAngle = PI means Under Feet
        // currentAngle = 3PI/2 means Up the Back

        float calculationAngle = currentAngle + Mathf.PI; // Offset currentAngle
                                                          // So when currentAngle = 0, calculationAngle = PI (UP)
                                                          // When currentAngle = PI/2, calculationAngle = 3PI/2 (FRONT/BACK)
                                                          // When currentAngle = PI, calculationAngle = 2PI or 0 (DOWN)
                                                          // When currentAngle = 3PI/2, calculationAngle = PI/2 (BACK/FRONT)


        // The vector representing the initial direction for the lowest point offset (straight down from swingPathCenter).
        Vector3 baseOffsetDirection = -characterTransform.up;

        // Rotate this base offset vector around the rotationAxis by the calculationAngle.
        Vector3 rotatedOffset = Quaternion.AngleAxis(Mathf.Rad2Deg * calculationAngle, rotationAxis) * (baseOffsetDirection * swingRadius);

        // The final calculated lowest point for the Bezier curve.
        Vector3 lowestPoint = swingPathCenter + rotatedOffset;

        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / (segments - 1);
            Vector3 position = (1 - t) * (1 - t) * ropeStart +
                               2 * (1 - t) * t * lowestPoint +
                               t * t * ropeEnd;
            lineRenderer.SetPosition(i, position);
        }
    }

    // Call this from an animation event
    // normalizedPhase = 0: Start of cycle (e.g., rope behind head, ready to swing forward)
    // normalizedPhase = 0.5: Mid-cycle (e.g., rope under feet)
    // normalizedPhase = 1.0: End of cycle (same as start)
    public void SyncRopeWithJump(float normalizedPhase)
    {
        // We want normalizedPhase = 0 (rope behind head) to map to currentAngle = 0 in our cycle definition above.
        // We want normalizedPhase = 0.5 (rope under feet) to map to currentAngle = PI.
        currentAngle = normalizedPhase * 2 * Mathf.PI;

        // If you uncomment the currentAngle accumulation in Update(), these events will take precedence
        // when they fire, resetting the angle.
    }
}
