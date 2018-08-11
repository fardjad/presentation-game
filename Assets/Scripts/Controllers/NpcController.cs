using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using Utils;
using Utils.StateMachine;
using Utils.StateMachine.Blackboard;
using Utils.StateMachine.Parser;
using Zenject;

namespace Controllers
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class NpcController : MonoBehaviour
    {
        private const float RotationTolerance = 1f;

        private const float KHalf = 0.5f;
        private Animator _animator;
        private CapsuleCollider _capsule;
        private Vector3 _capsuleCenter;
        private float _capsuleHeight;
        private bool _crouching;
        private float _forwardAmount;
        private Vector3 _groundNormal;
        private bool _isGrounded;
        private NavMeshAgent _navMeshAgent;
        private float _origGroundCheckDistance;
        private Rigidbody _rigidbody;
        private IStateMachine _stateMachine;
        private float _turnAmount;
        [SerializeField] public List<string> AnimationsStatesWithRootMotion = new List<string>();
        [SerializeField] public float AnimSpeedMultiplier = 1f;
        [Range(1f, 4f)] [SerializeField] public float GravityMultiplier = 2f;
        [SerializeField] public float GroundCheckDistance = 0.1f;
        [SerializeField] public float JumpPower = 12f;

        [SerializeField] public float MoveSpeedMultiplier = 1f;

        [SerializeField] public float MovingTurnSpeed = 360;

        //specific to the character in sample assets, will need to be modified to work with others
        [SerializeField] public float RunCycleLegOffset = 0.2f;
        [SerializeField] public float StationaryTurnSpeed = 180;
        public IBlackboard Blackboard { get; private set; }

        private void Awake()
        {
            Blackboard = new Blackboard();
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _capsule = GetComponent<CapsuleCollider>();
            _capsuleHeight = _capsule.height;
            _capsuleCenter = _capsule.center;

            // ReSharper disable BitwiseOperatorOnEnumWithoutFlags
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                                     RigidbodyConstraints.FreezeRotationZ;
            // ReSharper restore BitwiseOperatorOnEnumWithoutFlags

            _origGroundCheckDistance = GroundCheckDistance;

            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updatePosition = true;


            var stateMachineConfig = Resources.Load<TextAsset>("NpcStateMachine").text;
            var states = ConfigParser.GetStates(stateMachineConfig, Blackboard);
            _stateMachine = new StateMachine(states);

            Observable.FromEventPattern<EventHandler<StateEventArgs>, StateEventArgs>(h => h.Invoke,
                    h => _stateMachine.OnStateChanged += h,
                    h => _stateMachine.OnStateChanged -= h)
                .Select(e => e.EventArgs.State.Name)
                .DistinctUntilChanged()
                .Subscribe(Debug.Log);
        }


        public void Move(Vector3 move, bool crouch, bool jump, float speed = 0.5f, bool disableRotation = false)
        {
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (move.magnitude > 1f) move.Normalize();
            move = transform.InverseTransformDirection(move);
            CheckGroundStatus();
            move = Vector3.ProjectOnPlane(move, _groundNormal);
            _turnAmount = disableRotation ? 0 : Mathf.Atan2(move.x, move.z);
            _forwardAmount = move.z * speed;

            ApplyExtraTurnRotation();

            // control and velocity handling is different when grounded and airborne:
            if (_isGrounded)
                HandleGroundedMovement(crouch, jump);
            else
                HandleAirborneMovement();

            ScaleCapsuleForCrouching(crouch);
            PreventStandingInLowHeadroom();

            // send input and other state parameters to the animator
            UpdateAnimator(move);
        }

        public bool MoveTransform(Vector3 destination)
        {
            if ((transform.position - destination).magnitude <= 0.01f) return false;
            transform.position = Vector3.Lerp(transform.position, destination, 5f * Time.deltaTime);
            return true;
        }

        public bool RotateSmoothly(Quaternion desiredRotation)
        {
            if (Quaternion.Angle(transform.rotation, desiredRotation) < RotationTolerance) return false;

            transform.rotation =
                Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * 5f);
            return true;
        }

        private void ScaleCapsuleForCrouching(bool crouch)
        {
            if (_isGrounded && crouch)
            {
                if (_crouching) return;
                _capsule.height = _capsule.height / 2f;
                _capsule.center = _capsule.center / 2f;
                _crouching = true;
            }
            else
            {
                var crouchRay = new Ray(_rigidbody.position + Vector3.up * _capsule.radius * KHalf, Vector3.up);
                var crouchRayLength = _capsuleHeight - _capsule.radius * KHalf;
                if (Physics.SphereCast(crouchRay,
                    _capsule.radius * KHalf,
                    crouchRayLength,
                    Physics.AllLayers,
                    QueryTriggerInteraction.Ignore))
                {
                    _crouching = true;
                    return;
                }

                _capsule.height = _capsuleHeight;
                _capsule.center = _capsuleCenter;
                _crouching = false;
            }
        }

        private void PreventStandingInLowHeadroom()
        {
            // prevent standing up in crouch-only zones
            if (_crouching) return;
            var crouchRay = new Ray(_rigidbody.position + Vector3.up * _capsule.radius * KHalf, Vector3.up);
            var crouchRayLength = _capsuleHeight - _capsule.radius * KHalf;
            if (Physics.SphereCast(crouchRay,
                _capsule.radius * KHalf,
                crouchRayLength,
                Physics.AllLayers,
                QueryTriggerInteraction.Ignore))
                _crouching = true;
        }


        private void UpdateAnimator(Vector3 move)
        {
            // update the animator parameters
            _animator.SetFloat("Forward", _forwardAmount, 0.1f, Time.deltaTime);
            _animator.SetFloat("Turn", _turnAmount, 0.1f, Time.deltaTime);
            _animator.SetBool("Crouch", _crouching);
            _animator.SetBool("OnGround", _isGrounded);
            if (!_isGrounded) _animator.SetFloat("Jump", _rigidbody.velocity.y);

            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            var runCycle =
                Mathf.Repeat(
                    _animator.GetCurrentAnimatorStateInfo(0).normalizedTime + RunCycleLegOffset,
                    1);
            var jumpLeg = (runCycle < KHalf ? 1 : -1) * _forwardAmount;
            if (_isGrounded) _animator.SetFloat("JumpLeg", jumpLeg);

            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (_isGrounded && move.magnitude > 0)
                _animator.speed = AnimSpeedMultiplier;
            else
                _animator.speed = 1;
        }


        private void HandleAirborneMovement()
        {
            // apply extra gravity from multiplier:
            var extraGravityForce = Physics.gravity * GravityMultiplier - Physics.gravity;
            _rigidbody.AddForce(extraGravityForce);

            GroundCheckDistance = _rigidbody.velocity.y < 0 ? _origGroundCheckDistance : 0.01f;
        }


        private void HandleGroundedMovement(bool crouch, bool jump)
        {
            // check whether conditions are right to allow a jump:
            if (!jump || crouch || !_animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded")) return;
            // jump!
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, JumpPower, _rigidbody.velocity.z);
            _isGrounded = false;
            _animator.applyRootMotion = false;
            GroundCheckDistance = 0.1f;
        }

        private void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            var turnSpeed = Mathf.Lerp(StationaryTurnSpeed, MovingTurnSpeed, _forwardAmount);
            transform.Rotate(0, _turnAmount * turnSpeed * Time.deltaTime, 0);
        }


        private void OnAnimatorMove()
        {
            foreach (var stateName in AnimationsStatesWithRootMotion)
            {
                if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName)) continue;

                transform.position = _animator.rootPosition;
                transform.rotation = _animator.rootRotation;
                return;
            }

            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (!_isGrounded || !(Time.deltaTime > 0)) return;
            var v = _animator.deltaPosition * MoveSpeedMultiplier / Time.deltaTime;

            // we preserve the existing y part of the current velocity.
            v.y = _rigidbody.velocity.y;
            _rigidbody.velocity = v;
        }


        private void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(transform.position + Vector3.up * 0.1f,
                transform.position + Vector3.up * 0.1f + Vector3.down * GroundCheckDistance);
#endif
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            if (Physics.Raycast(transform.position + Vector3.up * 0.1f,
                Vector3.down,
                out hitInfo,
                GroundCheckDistance))
            {
                _groundNormal = hitInfo.normal;
                _isGrounded = true;
                _animator.applyRootMotion = true;
            }
            else
            {
                _isGrounded = false;
                _groundNormal = Vector3.up;
                _animator.applyRootMotion = false;
            }
        }

        private void Update()
        {
            _stateMachine.Tick(TimeSpan.FromSeconds(Time.deltaTime));

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (_stateMachine.CurrentState.Name)
            {
                case "SetDestination":
                case "WalkToTheChair":
                {
                    var chairWalkToPosition = (Vector3) Blackboard.Parameters["_chairWalkToPosition"];
                    _navMeshAgent.DrawChosenPath(Color.green);
                    if (_navMeshAgent.MoveTo(chairWalkToPosition, velocity => Move(velocity, false, false)))
                        Blackboard.Parameters["FinishedWalkToTheChair"] = "false";
                    else
                        Blackboard.Parameters["FinishedWalkToTheChair"] = "true";

                    break;
                }
                case "FaceForward":
                {
                    if (RotateSmoothly(Quaternion.Euler(0f, 90f, 0f)))
                        Blackboard.Parameters["FinishedFaceForward"] = "false";
                    else
                        Blackboard.Parameters["FinishedFaceForward"] = "true";

                    break;
                }
                case "DisableCollider":
                {
                    _capsule.enabled = false;
                    _rigidbody.detectCollisions = false;
                    _rigidbody.isKinematic = true;
                    _navMeshAgent.enabled = false;
                    break;
                }
                case "MoveToCenterOfTheChair":
                {
                    var chairCenter = (Vector3) Blackboard.Parameters["_chairCenter"];
                    _animator.SetBool("Seated", true);
                    var destination = VectorBuilder.FromVector(chairCenter)
                                          .SetY(transform.position.y)
                                          .ToVector() - transform.forward * 0.15f;
                    if (MoveTransform(destination))
                        Blackboard.Parameters["FinishedMoveToCenterOfTheChair"] = "false";
                    else
                        Blackboard.Parameters["FinishedMoveToCenterOfTheChair"] = "true";

                    break;
                }
                case "StandUp":
                {
                    var chairStandUpPosition = (Vector3) Blackboard.Parameters["_chairStandUpPosition"];
                    _animator.SetBool("Seated", false);
                    if (MoveTransform(chairStandUpPosition))
                        Blackboard.Parameters["FinishStandUp"] = "false";
                    else
                        Blackboard.Parameters["FinishStandUp"] = "true";

                    break;
                }
                case "EnableCollider":
                {
                    _navMeshAgent.enabled = true;
                    _navMeshAgent.SetDestination(transform.position);
                    _rigidbody.isKinematic = false;
                    _rigidbody.detectCollisions = true;
                    _capsule.enabled = true;
                    break;
                }
                case "ResetSitTrigger":
                {
                    var trigger = (StateMachineTrigger) Blackboard.Parameters["SitOnTheChair"];
                    if (trigger != null) trigger.Set();
                    break;
                }
                case "WalkToDestination":
                {
                    Blackboard.Parameters["FinishedWalkToDestination"] = "false";
                    var destination = (Vector3) Blackboard.Parameters["_destination"];
                    _navMeshAgent.DrawChosenPath(Color.green);
                    if (_navMeshAgent.MoveTo(destination,
                        velocity => Move(velocity, false, false)))
                        Blackboard.Parameters["FinishedWalkToDestination"] = "false";
                    else
                        Blackboard.Parameters["FinishedWalkToDestination"] = "true";

                    break;
                }
                case "ResetWalkTrigger":
                {
                    var trigger = (StateMachineTrigger) Blackboard.Parameters["WalkToDestination"];
                    if (trigger != null) trigger.Set();
                    break;
                }
                case "SittingOnTheChair":
                {
                    var trigger = (StateMachineTrigger) Blackboard.Parameters["SitOnTheChair"];
                    if (trigger != null) trigger.Set();
                    break;
                }
            }
        }

        [UsedImplicitly]
        public class Factory : PlaceholderFactory<NpcController>
        {
        }
    }
}