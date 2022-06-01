using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Generation;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Generation;
using Random = System.Random;

public class ComposableAI : BaseObject, IGenerator<int,float>
{
    public AIPattern[] Patterns;
    private AIPattern _currentPattern;
    private AIAction _currentAction;
    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(RunAI());
    }

    private IEnumerable<IEnumerable<Action>> RunAI()
    {
        while (isActiveAndEnabled)
        {
            while (Patterns == null || Patterns.Length == 0)
            {
                yield return TimeYields.WaitOneFrameX;
            }

            var patterns = Patterns
                .Where(p => p.Triggers.Length == 0 || p.Triggers.All(
                    t => t.CheckCondition() != t.Negate))
                .ToArray();

            var absolute = patterns.FirstOrDefault(p => p.IsAbsolute);

            var randomize = new WeightedDice<AIPattern>(patterns, this);
            _currentPattern = absolute ?? randomize.Generate();
            _currentAction = _currentPattern.Action;

            do
            {
                yield return _currentAction.Run().AsCoroutine();
                _currentAction = _currentAction.Next;
            } while (_currentAction != null);
        }
    }

    private static readonly Random Random = new Random();
    public int Seed { get; set; }
    public float Generate()
    {
        return (float)Random.NextDouble();
    }
}
