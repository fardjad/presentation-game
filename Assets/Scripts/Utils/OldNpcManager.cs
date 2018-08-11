// ReSharper disable All

using System;
using System.Collections.Generic;
using Controllers;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Utils
{
    internal enum SittingOnChairStage
    {
        SetDestination,
        WalkToTheChair,
        FaceForward,
        DisableCollider,
        MoveToCenterOfTheChair
    }

    internal enum NpcStateType
    {
        MoveTo,
        SitOnChair
    }

    internal class NpcState
    {
        public readonly IDictionary<string, object> Params; // UniRx does not support .NET Framework 4.x

        public NpcState(NpcStateType type)
        {
            Type = type;
            Params = new Dictionary<string, object>();
            Pending = true;
        }

        public NpcStateType Type { get; private set; }
        public bool Pending { get; set; }
    }

    [UsedImplicitly]
    public class OldNpcManager : ITickable, INpcManager
    {
        private readonly ChairManager _chairManager;
        private readonly IDictionary<string, NpcController> _npcControllerDictionary;
        private readonly IDictionary<string, NpcState> _npcStateDictionary;
        private int _col;

        private int _row;

        public OldNpcManager(ChairManager chairManager)
        {
            _chairManager = chairManager;
            _npcControllerDictionary = new Dictionary<string, NpcController>();
            _npcStateDictionary = new Dictionary<string, NpcState>();
        }

        public void RegisterNpcController(string id, NpcController npcController)
        {
            _npcControllerDictionary[id] = npcController;

            SitOnChair(id, _row, _col);

            _col += 1;
            if (_col == 4)
            {
                _col = 0;
                _row += 1;
            }

            if (_row == 2 && _col == 0)
            {
                _col += 1;
            }
        }

        public void Tick()
        {
            foreach (var pair in _npcControllerDictionary)
            {
                var id = pair.Key;
                var controller = pair.Value;

                if (!_npcStateDictionary.ContainsKey(id)) continue;
                var state = _npcStateDictionary[id];
                if (!state.Pending) continue;
                switch (state.Type)
                {
                    case NpcStateType.MoveTo:
                        break;
                    case NpcStateType.SitOnChair:
                        MoveToTick(id, controller, state.Params);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void MoveTo(string id, Vector3 destination)
        {
            var state = new NpcState(NpcStateType.MoveTo);
            state.Params["destination"] = destination;

            _npcStateDictionary[id] = state;
        }

        public void SitOnChair(string id, int row, int col)
        {
            var state = new NpcState(NpcStateType.SitOnChair);
            state.Params["row"] = row;
            state.Params["col"] = col;
            state.Params["stage"] = SittingOnChairStage.SetDestination;

            _npcStateDictionary[id] = state;
        }

        public void ResetState(string id)
        {
        }

        private void MoveToTick(string id, NpcController controller, IDictionary<string, object> stateParams)
        {
            var row = (int) stateParams["row"];
            var col = (int) stateParams["col"];
            var stage = (SittingOnChairStage) stateParams["stage"];

            var navMeshAgent = controller.GetComponent<NavMeshAgent>();
            var collider = controller.GetComponent<Collider>();
            var rigidbody = controller.GetComponent<Rigidbody>();
            var animator = controller.GetComponent<Animator>();


            switch (stage)
            {
                case SittingOnChairStage.SetDestination:
                    navMeshAgent.SetDestination(_chairManager.GetChairWalkToPosition(row, col));
                    stateParams["stage"] = SittingOnChairStage.WalkToTheChair;
                    break;
                case SittingOnChairStage.WalkToTheChair:
                    navMeshAgent.DrawChosenPath(Color.green);
                    if (!navMeshAgent.MoveTo(_chairManager.GetChairWalkToPosition(row, col),
                        velocity => controller.Move(velocity, false, false)))
                    {
                        stateParams["stage"] = SittingOnChairStage.DisableCollider;
                    }

                    break;
                case SittingOnChairStage.DisableCollider:
                    collider.enabled = false;
                    rigidbody.detectCollisions = false;
                    rigidbody.isKinematic = true;
                    navMeshAgent.enabled = false;
                    stateParams["stage"] = SittingOnChairStage.FaceForward;
                    break;
                case SittingOnChairStage.FaceForward:
                    if (!controller.RotateSmoothly(Quaternion.Euler(0f, 90f, 0f)))
                    {
                        stateParams["stage"] = SittingOnChairStage.MoveToCenterOfTheChair;
                    }

                    break;
                case SittingOnChairStage.MoveToCenterOfTheChair:
                    animator.SetBool("Seated", true);
                    var destination = VectorBuilder.FromVector(_chairManager.GetChairCenter(row, col))
                                          .SetY(controller.transform.position.y)
                                          .ToVector() - controller.transform.forward * 0.15f;
                    if (!controller.MoveTransform(destination))
                    {
                        _npcStateDictionary[id].Pending = false;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException("stage", stage, null);
            }
        }
    }
}
// ReSharper restore All