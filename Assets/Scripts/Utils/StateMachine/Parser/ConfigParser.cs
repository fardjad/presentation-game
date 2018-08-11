using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Utils.StateMachine.Blackboard;
using Utils.StateMachine.Conditions;

namespace Utils.StateMachine.Parser
{
    public static class ConfigParser
    {
        public static IEnumerable<IState> GetStates(string jsonString, IBlackboard blackboard)
        {
            var jsonConfig = (IDictionary<string, object>) JsonHelper.Deserialize(jsonString);
            var nodeList = (List<object>) jsonConfig["nodes"];
            var statesDictionary = new Dictionary<string, IState>();
            nodeList.ForEach(nodeObject =>
            {
                var node = (Dictionary<string, object>) nodeObject;
                var nodeId = (string) node["nodeId"];
                var name = (string) node["name"];
                var interruptible = (bool) node["interruptible"];
                var length = double.Parse(node["length"].ToString());
                var state = new State(name, interruptible, TimeSpan.FromMilliseconds(length));
                statesDictionary[nodeId] = state;
            });

            var transitionsList = (List<object>) jsonConfig["transitions"];
            transitionsList.ForEach(transitionObject =>
            {
                var transitionDictionary = (Dictionary<string, object>) transitionObject;
                var fromNodeId = (string) transitionDictionary["fromNodeId"];
                var toNodeId = (string) transitionDictionary["toNodeId"];
                var conditionDictionary = (Dictionary<string, object>) transitionDictionary["condition"];
                var conditionType = int.Parse(conditionDictionary["type"].ToString());
                var conditionParametersDictionary = (Dictionary<string, object>) conditionDictionary["parameters"];
                ICondition condition;
                switch (conditionType)
                {
                    case 0:
                        condition = new AlwaysTrueCondition();
                        break;
                    case 1:
                        condition = new CompareValue(blackboard,
                            (string) conditionParametersDictionary["parameterName"],
                            (string) conditionParametersDictionary["expectedValue"]);
                        break;
                    case 2:
                        condition = new ValueGreaterThan(blackboard,
                            (string) conditionParametersDictionary["parameterName"],
                            double.Parse(conditionParametersDictionary["lowerBound"].ToString()));
                        break;
                    case 3:
                        condition = new ValueLessThan(blackboard,
                            (string) conditionParametersDictionary["parameterName"],
                            double.Parse(conditionParametersDictionary["upperBound"].ToString()));
                        break;
                    case 4:
                        condition = new ValueInRange(blackboard,
                            (string) conditionParametersDictionary["parameterName"],
                            double.Parse(conditionParametersDictionary["lowerBound"].ToString()),
                            double.Parse(conditionParametersDictionary["upperBound"].ToString()));
                        break;
                    case 5:
                        condition = new CheckTrigger(blackboard,
                            (string) conditionParametersDictionary["parameterName"]);
                        break;
                    default:
                        throw new JsonException("Invalid condition type");
                }

                var transition = new Transition(condition, statesDictionary[toNodeId]);
                statesDictionary[fromNodeId].Transitions.Add(transition);
            });
            return statesDictionary.Values;
        }
    }
}
