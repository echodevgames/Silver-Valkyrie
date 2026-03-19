using UnityEngine;

/// <summary>
/// Draugr enemy controller. Wanders the playfield using 8-directional sprite animation
/// driven purely in code — no Animator required. Destroyed on first contact with the ball.
///
/// Assign the four sprites for each direction from the Draugrsheet sub-assets.
/// Adjust wanderMin / wanderMax to match the playable area bounds of your scene.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class DraugrEnemy : MonoBehaviour
{
    private const string BallTag = "Ball";

    [Header("Walk Animations — 4 frames each, assigned from Draugrsheet sub-assets")]
    [SerializeField] private Sprite[] walkDown;
    [SerializeField] private Sprite[] walkDownLeft;
    [SerializeField] private Sprite[] walkLeft;
    [SerializeField] private Sprite[] walkUpLeft;
    [SerializeField] private Sprite[] walkUp;
    [SerializeField] private Sprite[] walkUpRight;
    [SerializeField] private Sprite[] walkRight;
    [SerializeField] private Sprite[] walkDownRight;

    [Header("Movement")]
    [Tooltip("World-unit speed while wandering.")]
    [SerializeField] private float moveSpeed = 1.2f;

    [Tooltip("Distance to waypoint considered 'reached'.")]
    [SerializeField] private float waypointThreshold = 0.3f;

    [Tooltip("Maximum seconds before a new waypoint is picked even if the current one is not yet reached.")]
    [SerializeField] private float waypointTimeout = 4f;

    [Tooltip("Bottom-left corner of the wander area in world space. Match to playfield bounds.")]
    [SerializeField] private Vector2 wanderMin = new Vector2(-2.5f, -5.0f);

    [Tooltip("Top-right corner of the wander area in world space. Match to playfield bounds.")]
    [SerializeField] private Vector2 wanderMax = new Vector2(2.5f, 5.0f);

    [Header("Animation")]
    [Tooltip("Sprite frames played per second.")]
    [SerializeField] private float framesPerSecond = 8f;

    // ── Private ───────────────────────────────────────────────────────────────

    private SpriteRenderer _renderer;
    private Rigidbody2D    _rb;

    private Sprite[] _activeAnim;
    private int      _frameIndex;
    private float    _frameTimer;

    private Vector2 _waypoint;
    private float   _waypointTimer;

    private Vector2 _lastPosition;
    private float   _stuckTimer;

    private const float StuckCheckInterval = 1.0f;
    private const float StuckDistanceMin   = 0.05f;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        _renderer   = GetComponent<SpriteRenderer>();
        _rb         = GetComponent<Rigidbody2D>();
        _activeAnim = walkDown;
    }

    private void Start()
    {
        _lastPosition = _rb.position;
        _stuckTimer   = StuckCheckInterval;
        PickNewWaypoint();
    }

    private void Update()
    {
        AdvanceFrame();
    }

    private void FixedUpdate()
    {
        MoveTowardWaypoint();
        CheckStuck();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(BallTag))
            Destroy(gameObject);
    }

    // ── Movement ──────────────────────────────────────────────────────────────

    private void MoveTowardWaypoint()
    {
        Vector2 pos      = _rb.position;
        Vector2 delta    = _waypoint - pos;
        float   distance = delta.magnitude;

        _waypointTimer -= Time.fixedDeltaTime;

        if (distance < waypointThreshold || _waypointTimer <= 0f)
        {
            PickNewWaypoint();
            return;
        }

        Vector2 dir = delta / distance;
        _rb.linearVelocity = dir * moveSpeed;
        SetAnimationForDirection(dir);
    }

    private void CheckStuck()
    {
        _stuckTimer -= Time.fixedDeltaTime;
        if (_stuckTimer > 0f) return;

        _stuckTimer = StuckCheckInterval;

        if (Vector2.Distance(_rb.position, _lastPosition) < StuckDistanceMin)
            PickNewWaypoint();

        _lastPosition = _rb.position;
    }

    private void PickNewWaypoint()
    {
        _waypoint = new Vector2(
            Random.Range(wanderMin.x, wanderMax.x),
            Random.Range(wanderMin.y, wanderMax.y)
        );
        _waypointTimer = waypointTimeout;
    }

    // ── Animation ─────────────────────────────────────────────────────────────

    private void SetAnimationForDirection(Vector2 dir)
    {
        Sprite[] next = ResolveAnimation(dir);
        if (next == _activeAnim || next == null || next.Length == 0) return;

        _activeAnim = next;
        _frameIndex = 0;
        _frameTimer = 0f;
    }

    /// <summary>
    /// Converts a movement direction vector to an 8-directional animation set.
    /// Angle 0° = right, 90° = up, measured CCW.
    /// </summary>
    private Sprite[] ResolveAnimation(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;

        // Snap to the nearest of 8 sectors (45° each)
        int sector = Mathf.RoundToInt(angle / 45f) % 8;

        return sector switch
        {
            0 => walkRight,
            1 => walkUpRight,
            2 => walkUp,
            3 => walkUpLeft,
            4 => walkLeft,
            5 => walkDownLeft,
            6 => walkDown,
            7 => walkDownRight,
            _ => walkDown
        };
    }

    private void AdvanceFrame()
    {
        if (_activeAnim == null || _activeAnim.Length == 0) return;

        _frameTimer += Time.deltaTime;
        float frameDuration = 1f / Mathf.Max(framesPerSecond, 0.1f);

        if (_frameTimer < frameDuration) return;

        _frameTimer -= frameDuration;
        _frameIndex  = (_frameIndex + 1) % _activeAnim.Length;
        _renderer.sprite = _activeAnim[_frameIndex];
    }
}
