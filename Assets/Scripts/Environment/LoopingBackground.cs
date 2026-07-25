using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private SpriteRenderer _leftSprite;
    [SerializeField] private SpriteRenderer _centerSprite;
    [SerializeField] private SpriteRenderer _rightSprite;

    [Header("Settings")]
    [Tooltip("Small overlap used to hide tiny seams between sprites.")]
    [SerializeField] private float _seamOverlap = 0.01f;

    private float _spriteWidth;

    private void Awake()
    {
        if (_cameraTransform == null && Camera.main != null)
        {
            _cameraTransform = Camera.main.transform;
        }

        if (!ReferencesAreValid())
        {
            enabled = false;
            return;
        }

        _spriteWidth = _centerSprite.bounds.size.x;

        ArrangeStartingSprites();
    }

    private void LateUpdate()
    {
        if (_cameraTransform == null)
            return;

        RecycleSprites();
    }

    private bool ReferencesAreValid()
    {
        if (_leftSprite == null ||
            _centerSprite == null ||
            _rightSprite == null)
        {
            Debug.LogError(
                $"{name}: One or more background sprite references are missing.",
                this
            );

            return false;
        }

        if (_cameraTransform == null)
        {
            Debug.LogError(
                $"{name}: No camera was assigned and no Main Camera was found.",
                this
            );

            return false;
        }

        return true;
    }

    private void ArrangeStartingSprites()
    {
        float spacing = _spriteWidth - _seamOverlap;

        Vector3 centerPosition = _centerSprite.transform.localPosition;

        _leftSprite.transform.localPosition = new Vector3(
            centerPosition.x - spacing,
            centerPosition.y,
            centerPosition.z
        );

        _rightSprite.transform.localPosition = new Vector3(
            centerPosition.x + spacing,
            centerPosition.y,
            centerPosition.z
        );

        _centerSprite.flipX = false;
        _leftSprite.flipX = !_centerSprite.flipX;
        _rightSprite.flipX = !_centerSprite.flipX;
    }

    private void RecycleSprites()
    {
        float halfWidth = _spriteWidth * 0.5f;
        float cameraX = _cameraTransform.position.x;

        // Camera moved far enough to the right that the left sprite
        // can be recycled to the right side.
        if (cameraX > _centerSprite.bounds.center.x + halfWidth)
        {
            MoveLeftSpriteToRight();
        }
        // Camera moved far enough to the left that the right sprite
        // can be recycled to the left side.
        else if (cameraX < _centerSprite.bounds.center.x - halfWidth)
        {
            MoveRightSpriteToLeft();
        }
    }

    private void MoveLeftSpriteToRight()
    {
        float spacing = _spriteWidth - _seamOverlap;

        SpriteRenderer oldLeft = _leftSprite;
        SpriteRenderer oldCenter = _centerSprite;
        SpriteRenderer oldRight = _rightSprite;

        oldLeft.transform.position = new Vector3(
            oldRight.transform.position.x + spacing,
            oldLeft.transform.position.y,
            oldLeft.transform.position.z
        );

        // Continue the alternating mirrored pattern.
        oldLeft.flipX = !oldRight.flipX;

        _leftSprite = oldCenter;
        _centerSprite = oldRight;
        _rightSprite = oldLeft;
    }

    private void MoveRightSpriteToLeft()
    {
        float spacing = _spriteWidth - _seamOverlap;

        SpriteRenderer oldLeft = _leftSprite;
        SpriteRenderer oldCenter = _centerSprite;
        SpriteRenderer oldRight = _rightSprite;

        oldRight.transform.position = new Vector3(
            oldLeft.transform.position.x - spacing,
            oldRight.transform.position.y,
            oldRight.transform.position.z
        );

        // Continue the alternating mirrored pattern.
        oldRight.flipX = !oldLeft.flipX;

        _rightSprite = oldCenter;
        _centerSprite = oldLeft;
        _leftSprite = oldRight;
    }
}