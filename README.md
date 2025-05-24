# Simple Procedural Unity Line Renderer Jump Rope

![Image](https://github.com/user-attachments/assets/cf9aa126-180a-409a-b72b-3a6ab3fc3406)

A simple Unity project demonstrating how to create a dynamic jump rope effect using a Line Renderer and C# scripting. This project shows how to:
- Draw a rope between two points (e.g., character's hands).
- Simulate a swinging motion for the rope.
- Make the rope sag realistically using a Bezier curve.
- Synchronize the rope's movement with character animations using Animation Events (optional setup included).

## Features

- **Procedural Rope:** Rope is drawn and animated via script using Unity's Line Renderer.
- **Customizable:** Rope segments, sag, swing speed, and swing radius are adjustable.
- **Bezier Curve:** Provides a smooth arc for the rope.
- **Animation Synchronization:** Includes an example setup for using Animation Events to control the rope's cycle.
- **Simple Demo Scene:** Includes a basic character setup to showcase the effect.

## Getting Started

### Prerequisites

- Unity 2020.3 LTS or later (or specify your version).

## How It Works

### 1. RopeController.cs

This script is attached to an empty GameObject (e.g., "JumpRope") which also has a `LineRenderer` component.

-   **Hand Holds:** It takes two `Transform` references (`leftHandHold` and `rightHandHold`) which represent the points where the rope is held. These should be parented to your character's hand bones or animated GameObjects.
-   **Character Transform:** A reference to the main character's transform is used for relative calculations like `characterTransform.right` for the swing axis.
-   **Line Renderer:** The script dynamically updates the positions of the Line Renderer to draw the rope.
-   **Swinging Logic:**
    -   The `currentAngle` variable is incremented over time by `swingSpeed` to create a continuous rotation.
    -   The rope's lowest point (`lowestPoint`) is calculated to orbit around a `swingPathCenter` (a point below the midpoint of the hands, adjusted by `ropeSag`).
    -   The `rotationAxis` is set to `characterTransform.right`, making the rope swing in the character's local YZ plane (over the head, under the feet).
    -   A phase shift (`calculationAngle = currentAngle + Mathf.PI`) is used to ensure `currentAngle = 0` corresponds to the rope being at the top of its swing (over the head/back).
-   **Bezier Curve:** A quadratic Bezier curve is used to draw a smooth arc for the rope using the start hand, end hand, and the calculated `lowestPoint`.
    `P(t) = (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2`
    where `P0` is `leftHandHold.position`, `P1` is `lowestPoint`, and `P2` is `rightHandHold.position`.

### Setting up Animation
**Example for Mixamo Users:**
1. Download a character and a jump rope animation (e.g., "Jump Rope") from [Mixamo.com](https://www.mixamo.com).
2. Import them into your Unity project.
3. Replace the "Capsule" in the DemoScene with your Mixamo character.
4. Find the hand bones in your character's hierarchy (e.g., `mixamorig:LeftHand`, `mixamorig:RightHand`).
5. Create or move the "LeftHandHold" and "RightHandHold" GameObjects to be children of these respective hand bones and adjust their local positions so they align with where the character would hold the rope handles.
6. Assign your jump rope animation to the Animator on your character.
7. Set up Animation Events on your character's jump rope animation clip as described above.

## Customization

You can adjust the following public variables in the `RopeController` script in the Inspector:

-   **Left Hand Hold / Right Hand Hold:** Assign your character's hand transforms.
-   **Character Transform:** Assign your main character object.
-   **Segments:** Number of points in the Line Renderer (more segments = smoother rope, but more computation).
-   **Rope Sag:** How much the middle of the rope hangs down.
-   **Swing Speed:** How fast the rope rotates when not driven by animation events.
-   **Swing Radius:** The radius of the circle the rope's lowest point makes.

## License

This project is licensed under the MIT License 
## Acknowledgements 
-   Created Using Google Gemini
