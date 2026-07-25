using UnityEngine;

[DefaultExecutionOrder(1000)]
[DisallowMultipleComponent]
public class InfiniteMirroredParallax : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _targetCamera;
    [SerializeField] private SpriteRenderer _originalSprite;

    [Header("Parallax")]
    [Range(0f, 1f)]
    [Tooltip("0 = fixed in world space. 1 = follows the camera exactly.")]
    [SerializeField] private float _horizontalFollow = 0.1f;

    [Range(0f, 1f)]
    [Tooltip("0 = fixed vertically. 1 = follows the camera exactly.")]
    [SerializeField] private float _verticalFollow = 0f;

    [Header("Looping")]
    [SerializeField]
    [Tooltip("Repeat the sprite vertically as well as horizontally.")]
    private bool _loopVertically = false;

    [Min(0f)]
    [SerializeField] private float _horizontalSeamOverlap = 0.02f;

    [Min(0f)]
    [SerializeField] private float _verticalSeamOverlap = 0.02f;

    [Min(0)]
    [SerializeField] private int _extraColumnsPerSide = 1;

    [Min(0)]
    [SerializeField] private int _extraRowsPerSide = 1;

    private SpriteRenderer[,] _pieces;

    private Vector3 _startingCameraPosition;
    private Vector3 _startingSpritePosition;

    private float _pieceWidth;
    private float _pieceHeight;
    private float _horizontalSpacing;
    private float _verticalSpacing;

    private int _columnCount;
    private int _rowCount;

    private bool _originalFlipX;
    private bool _originalFlipY;

    private void Start()
    {
        ResolveReferences();

        if (!ValidateSetup())
        {
            enabled = false;
            return;
        }

        _startingCameraPosition = _targetCamera.transform.position;
        _startingSpritePosition = _originalSprite.transform.position;

        _pieceWidth = _originalSprite.bounds.size.x;
        _pieceHeight = _originalSprite.bounds.size.y;

        if (_pieceWidth <= 0f || _pieceHeight <= 0f)
        {
            Debug.LogError(
                $"{name}: The assigned sprite has invalid bounds.",
                this
            );

            enabled = false;
            return;
        }

        _horizontalSeamOverlap = Mathf.Clamp(
            _horizontalSeamOverlap,
            0f,
            _pieceWidth * 0.25f
        );

        _verticalSeamOverlap = Mathf.Clamp(
            _verticalSeamOverlap,
            0f,
            _pieceHeight * 0.25f
        );

        _horizontalSpacing =
            _pieceWidth - _horizontalSeamOverlap;

        _verticalSpacing =
            _pieceHeight - _verticalSeamOverlap;

        _originalFlipX = _originalSprite.flipX;
        _originalFlipY = _originalSprite.flipY;

        CalculateGridSize();
        CreatePieces();
        UpdatePiecePositions();
    }

    private void LateUpdate()
    {
        UpdatePiecePositions();
    }

    private void ResolveReferences()
    {
        if (_targetCamera == null)
        {
            _targetCamera = Camera.main;
        }

        if (_originalSprite == null)
        {
            _originalSprite =
                GetComponentInChildren<SpriteRenderer>();
        }
    }

    private bool ValidateSetup()
    {
        if (_targetCamera == null)
        {
            Debug.LogError(
                $"{name}: No camera was assigned and no Main Camera was found.",
                this
            );

            return false;
        }

        if (!_targetCamera.orthographic)
        {
            Debug.LogError(
                $"{name}: This component requires an orthographic camera.",
                this
            );

            return false;
        }

        if (_originalSprite == null)
        {
            Debug.LogError(
                $"{name}: No SpriteRenderer was assigned or found.",
                this
            );

            return false;
        }

        if (_originalSprite.transform.parent != transform)
        {
            Debug.LogError(
                $"{name}: The original sprite must be a direct child of this object.",
                this
            );

            return false;
        }

        return true;
    }

    private void CalculateGridSize()
    {
        float cameraHeight =
            _targetCamera.orthographicSize * 2f;

        float cameraWidth =
            cameraHeight * _targetCamera.aspect;

        int visibleColumns =
            Mathf.CeilToInt(cameraWidth / _horizontalSpacing) + 1;

        _columnCount =
            visibleColumns + (_extraColumnsPerSide * 2);

        _columnCount = MakeOddAtLeastThree(_columnCount);

        if (_loopVertically)
        {
            int visibleRows =
                Mathf.CeilToInt(cameraHeight / _verticalSpacing) + 1;

            _rowCount =
                visibleRows + (_extraRowsPerSide * 2);

            _rowCount = MakeOddAtLeastThree(_rowCount);
        }
        else
        {
            _rowCount = 1;
        }
    }

    private static int MakeOddAtLeastThree(int value)
    {
        value = Mathf.Max(value, 3);

        if (value % 2 == 0)
        {
            value++;
        }

        return value;
    }

    private void CreatePieces()
    {
        _pieces =
            new SpriteRenderer[_columnCount, _rowCount];

        int centerColumn = _columnCount / 2;
        int centerRow = _rowCount / 2;

        for (int column = 0; column < _columnCount; column++)
        {
            for (int row = 0; row < _rowCount; row++)
            {
                bool isCenter =
                    column == centerColumn &&
                    row == centerRow;

                SpriteRenderer piece;

                if (isCenter)
                {
                    piece = _originalSprite;
                }
                else
                {
                    piece = Instantiate(
                        _originalSprite,
                        transform
                    );

                    piece.name =
                        $"{_originalSprite.name}_Loop_{column}_{row}";
                }

                _pieces[column, row] = piece;
            }
        }
    }

    private void UpdatePiecePositions()
    {
        Vector3 cameraPosition =
            _targetCamera.transform.position;

        Vector3 cameraDisplacement =
            cameraPosition - _startingCameraPosition;

        float parallaxOriginX =
            _startingSpritePosition.x +
            cameraDisplacement.x * _horizontalFollow;

        float parallaxOriginY =
            _startingSpritePosition.y +
            cameraDisplacement.y * _verticalFollow;

        int centerWorldColumn = Mathf.RoundToInt(
            (cameraPosition.x - parallaxOriginX) /
            _horizontalSpacing
        );

        int centerWorldRow = 0;

        if (_loopVertically)
        {
            centerWorldRow = Mathf.RoundToInt(
                (cameraPosition.y - parallaxOriginY) /
                _verticalSpacing
            );
        }

        int centerArrayColumn = _columnCount / 2;
        int centerArrayRow = _rowCount / 2;

        for (int column = 0; column < _columnCount; column++)
        {
            int columnOffset =
                column - centerArrayColumn;

            int worldColumn =
                centerWorldColumn + columnOffset;

            for (int row = 0; row < _rowCount; row++)
            {
                int rowOffset =
                    row - centerArrayRow;

                int worldRow = _loopVertically
                    ? centerWorldRow + rowOffset
                    : 0;

                SpriteRenderer piece =
                    _pieces[column, row];

                piece.transform.position = new Vector3(
                    parallaxOriginX +
                    worldColumn * _horizontalSpacing,

                    parallaxOriginY +
                    worldRow * _verticalSpacing,

                    _startingSpritePosition.z
                );

                // Alternate normal and mirrored images.
                bool alternateHorizontal =
                    Mathf.Abs(worldColumn) % 2 == 1;

                piece.flipX =
                    _originalFlipX ^ alternateHorizontal;

                bool alternateVertical =
                    Mathf.Abs(worldRow) % 2 == 1;

                piece.flipY =
                    _originalFlipY ^ alternateVertical;
            }
        }
    }
}