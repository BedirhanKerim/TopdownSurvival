# Player System — Spec

Status: **movement + facing + 8-way locomotion DONE.** Auto-fire, the 2-layer
upper/lower-body mask split, and the `speed` blend parameter are deferred.

## 1. Purpose
Top-down twin-stick hero. The Floating joystick controls movement on the X/Z plane.
The player **auto-rotates to face the nearest enemy** and moves **independently of
facing**, so it can strafe or walk backward while still aiming. Locomotion is shown by
an 8-way directional blend tree.

## 2. Locked decisions
- No-target facing: face the movement direction; keep last facing when idle.
- Speed model: constant full speed above the dead zone (not analog magnitude-scaled).
- Joystick: Floating (Joystick Pack), thumb-anywhere.
- Rotation: `Quaternion.RotateTowards` (constant deg/sec, frame-rate independent).
- Motor: `CharacterController` (infinite mass vs swarming enemies, collide-and-slide).
- No VContainer registration for player pieces — scene components wired by serialized refs.
- Rigs are Humanoid; clips share the player avatar (Copy From Other Avatar).

## 3. Scripts (`Assets/_Project/Scripts/`)
| File | Type | Responsibility |
|------|------|----------------|
| `Input/IMovementInput.cs` | interface | `Vector2 Move` (x = world +X right, y = world +Z forward, 0..1). |
| `Input/JoystickMovementInput.cs` | MonoBehaviour : IMovementInput | Wraps the global `Joystick` type → `Direction`; own dead zone; never leaks the package type. |
| `Combat/IAimTargetProvider.cs` | interface | `Transform Current`, `Vector3 AimDirection`, `bool TryGetTarget(out Transform)`. Shared by facing now and fire later. |
| `Combat/EnemyTargeting.cs` | MonoBehaviour : IAimTargetProvider | Throttled, alloc-free nearest-enemy search. |
| `Player/PlayerController.cs` | MonoBehaviour | Motor + facing + animator driving. |

`PlayerController` casts serialized `m_MovementInputSource` / `m_AimProviderSource`
(MonoBehaviour) to their interfaces in `Awake`; `m_Controller` and `m_Animator` are
serialized. All null-checked with `Debug.LogError`.

## 4. Behavior (PlayerController.Update, all `Time.deltaTime`)
1. **Read input** → `worldMove = new Vector3(input.x, 0, input.y).normalized` when
   `input.magnitude >= m_MoveDeadzone`, else zero. World-space (camera is straight
   top-down, so camera-relative == world).
2. **Motor** → horizontal velocity = `worldMove * m_MaxSpeed`; gravity accumulates into
   `m_VerticalVelocity` (clamped to -2 when grounded); `CharacterController.Move`.
   Movement is independent of rotation (decoupling at the motor).
3. **Facing** → desired = `AimDirection` when a target exists, else `worldMove`
   (FaceMoveDirection) else skip. Flatten Y, `LookRotation`, `RotateTowards` at
   `m_TurnSpeedDegPerSec`. The **root** rotates; the mesh child stays local-identity.
4. **Animator** → `localMove = transform.InverseTransformDirection(worldMove)` expresses
   movement relative to facing. `moveX = localMove.x` (strafe), `moveZ = localMove.z`
   (fwd/back). Facing an enemy + pushing back ⇒ `moveZ ≈ -1` ⇒ backward clip. Set with
   damping (`m_LocomotionDamp`); param hashes cached as `static readonly int`.

### EnemyTargeting
Preallocated `Collider[]` (size `m_MaxResults`). Search throttled to `m_RefreshInterval`
via `Physics.OverlapSphereNonAlloc` on `m_EnemyMask`; nearest picked by squared XZ
distance in a plain `for` loop (no LINQ). Aim direction recomputed every frame from the
live target so facing tracks a moving enemy smoothly between searches.

> Targeting requires enemies on the **Enemy layer** with a collider, and
> `m_EnemyMask = Enemy only`. A mask that includes the ground/player makes the player
> chase a stray world point and the blend values swing — this was an early test bug.

## 5. Tunables (serialized, defaults)
`m_MaxSpeed` 3.5 · `m_MoveDeadzone` 0.15 · `m_Gravity` -20 · `m_TurnSpeedDegPerSec` 720 ·
`m_LocomotionDamp` 0.1 · `m_NoTargetFacing` FaceMoveDirection · `EnemyTargeting`:
`m_DetectRadius` 20, `m_RefreshInterval` 0.1, `m_MaxResults` 32, `m_DeadZone` (joystick) 0.15.

## 6. Animator (`AC_Player.controller`)
**As built (test):** single Base layer, default state **Locomotion** = 2D Freeform
Directional blend tree on `moveX`/`moveZ`, center (0,0) = `A_RifleAimingIdle`. Apply Root
Motion off. 9 clips on the unit circle (diagonals at 0.707):

| Clip | (moveX, moveZ) |
|------|----------------|
| A_RifleAimingIdle | (0, 0) |
| A_WalkForward | (0, 1) |
| A_WalkForwardRight | (0.707, 0.707) |
| A_WalkRight | (1, 0) |
| A_WalkBackwardRight | (0.707, -0.707) |
| A_WalkingBackwards | (0, -1) |
| A_WalkBackwardLeft | (-0.707, -0.707) |
| A_WalkLeft | (-1, 0) |
| A_WalkForwardLeft | (-0.707, 0.707) |

**Deferred — 2-layer split:** Layer 0 Lower Body (mask `AM_LowerBody`) = the locomotion
tree; Layer 1 Upper Body (mask `AM_UpperBody`, Override, weight 1) = AimIdle
(`A_RifleAimingIdle`) ↔ Fire (`A_FiringRifle`) gated by `isFiring`. Legs walk, torso
always aims, arms fire.

**Deferred — `speed` param:** optional outer 1D blend (speed: 0→Idle, 1→2D directional
tree) for a crisper idle↔walk blend. The code already writes `speed = worldMove.magnitude`;
the param is simply absent for now (Unity ignores SetFloat on a missing param).

## 7. Prefab / scene
`P_Player`: root (Transform 0/0/0) with CharacterController + PlayerController +
EnemyTargeting + JoystickMovementInput; child SK_Player mesh (local identity) with Animator
→ AC_Player (Apply Root Motion off); SM_Rifle on the right-hand bone. The Floating joystick
lives on a Screen-Space Canvas in the scene (EventSystems-based, not in the prefab); wire
`JoystickMovementInput.m_Joystick` to it. Top-down camera. Ground plane with a collider.

## 8. Verification
Play: joystick moves the player on X/Z; with an enemy (Enemy layer, collider, in radius)
the player rotates to face it and the 8-way blend plays forward/back/strafe correctly
while still aiming; with no enemy it faces the move direction and idles when stopped.
`EnemyTargeting.Current` (Debug inspector) shows the enemy when in range, null otherwise.
Profiler: no GC alloc spikes in `Update`.
