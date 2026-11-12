// Created by ChatGPT for the Shrimp Game project.
// Automatically draws dialogue above the NPC when the player is nearby.
// Does not require TextMeshPro, a Canvas, or a background object.

using UnityEngine;

public class NPCAdvice : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Transform player;

    // Leave empty to use the camera tagged MainCamera.
    [SerializeField] private Camera gameCamera;

    [Header("Talking Distance")]
    [SerializeField, Min(0f)] private float talkDistance = 3f;

    [Header("Dialogue Position")]
    // World position offset above the NPC.
    [SerializeField] private Vector3 worldOffset =
        new Vector3(0f, 1.5f, 0f);

    // Screen position adjustment in pixels.
    // Positive Y moves the box downward.
    [SerializeField] private Vector2 screenOffset = Vector2.zero;

    [Header("Dialogue Appearance")]
    // Width and height in screen pixels.
    [SerializeField] private Vector2 boxSize =
        new Vector2(360f, 100f);

    [SerializeField, Min(1)] private int fontSize = 20;
    [SerializeField, Min(0)] private int padding = 12;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color backgroundColor = Color.black;

    [Header("Dialogue Settings")]
    [SerializeField, Min(0.1f)] private float secondsPerSentence = 4f;
    [SerializeField] private bool loopDialogue = true;

    [Header("Dialogue Draw Order")]
    [Tooltip("Lower values draw in front of other OnGUI elements.")]
    [SerializeField] private int boxDrawOrder = -100;

    [SerializeField, TextArea(2, 5)]
    private string[] sentences =
    {
        "Easy there, youngster. I retired from fighting!",
        "Keep moving. Don't let the purple shrimp surround you.",
        "Collect crystals from defeated enemies.",
        "Good luck out there. I'll guard this corner."
    };

    private bool isTalking;
    private int sentenceIndex;
    private float sentenceTimer;
    private GUIStyle textStyle;

    private void Awake()
    {
        if (gameCamera == null)
        {
            gameCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (player == null ||
            !player.gameObject.activeInHierarchy ||
            sentences == null ||
            sentences.Length == 0)
        {
            ResetDialogue();
            return;
        }

        // Check distance on the XY plane for the 2D game.
        Vector2 difference =
            (Vector2)player.position - (Vector2)transform.position;

        bool playerNearby =
            difference.sqrMagnitude <= talkDistance * talkDistance;

        if (!playerNearby)
        {
            ResetDialogue();
            return;
        }

        // Start the dialogue when the player enters the talking range.
        if (!isTalking)
        {
            isTalking = true;
            sentenceIndex = 0;
            sentenceTimer = 0f;
            return;
        }

        // Keep the final sentence visible when looping is disabled.
        if (!loopDialogue && sentenceIndex >= sentences.Length - 1)
        {
            return;
        }

        sentenceTimer += Time.deltaTime;

        if (sentenceTimer >= Mathf.Max(0.1f, secondsPerSentence))
        {
            sentenceTimer = 0f;
            sentenceIndex++;

            if (sentenceIndex >= sentences.Length)
            {
                sentenceIndex = 0;
            }
        }
    }

    private void OnGUI()
    {
        if (!isTalking ||
            sentences == null ||
            sentenceIndex >= sentences.Length)
        {
            return;
        }

        if (gameCamera == null)
        {
            gameCamera = Camera.main;

            if (gameCamera == null)
            {
                return;
            }
        }

        // Convert the NPC's world position into a screen position.
        Vector3 screenPosition =
            gameCamera.WorldToScreenPoint(transform.position + worldOffset);

        // Do not draw dialogue behind the camera.
        if (screenPosition.z <= 0f)
        {
            return;
        }

        float width = Mathf.Max(40f, boxSize.x);
        float height = Mathf.Max(30f, boxSize.y);

        // GUI coordinates start at the top-left of the screen.
        // The bottom center of the box sits at the offset position.
        Rect boxRect = new Rect(
            screenPosition.x - width * 0.5f + screenOffset.x,
            Screen.height - screenPosition.y - height + screenOffset.y,
            width,
            height
        );

        // Keep the box inside the game window.
        boxRect.x = Mathf.Clamp(
            boxRect.x, 0f, Mathf.Max(0f, Screen.width - width));

        boxRect.y = Mathf.Clamp(
            boxRect.y, 0f, Mathf.Max(0f, Screen.height - height));

        // Create a centered text style without any text components.
        if (textStyle == null)
        {
            textStyle = new GUIStyle(GUI.skin.label);
        }

        textStyle.fontSize = fontSize;
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.wordWrap = true;
        textStyle.richText = false;
        textStyle.normal.textColor = textColor;

        // Save GUI settings so other scripts are unaffected.
        Color previousColor = GUI.color;
        int previousDepth = GUI.depth;

        GUI.depth = boxDrawOrder;
        GUI.color = backgroundColor;

        // Draw a solid background using Unity's built-in white texture.
        GUI.DrawTexture(boxRect, Texture2D.whiteTexture);

        GUI.color = Color.white;

        float inset = Mathf.Clamp(
            padding, 0f, Mathf.Min(width, height) * 0.5f - 1f);

        Rect textRect = new Rect(
            boxRect.x + inset,
            boxRect.y + inset,
            boxRect.width - inset * 2f,
            boxRect.height - inset * 2f
        );

        GUI.Label(textRect, sentences[sentenceIndex], textStyle);

        GUI.color = previousColor;
        GUI.depth = previousDepth;
    }

    private void ResetDialogue()
    {
        isTalking = false;
        sentenceIndex = 0;
        sentenceTimer = 0f;
    }

    private void OnDisable()
    {
        ResetDialogue();
    }

    private void OnDrawGizmosSelected()
    {
        // Show the talking range in the Scene view.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, talkDistance);
    }
}