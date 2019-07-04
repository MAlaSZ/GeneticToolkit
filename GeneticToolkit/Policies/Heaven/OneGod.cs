﻿using GeneticToolkit.Interfaces;
using JetBrains.Annotations;

namespace GeneticToolkit.Policies.Heaven
{
    [PublicAPI]
    public class OneGod : IHeavenPolicy
    {
        public IIndividual[] Memory { get; } = new IIndividual[1];

        public void HandleGeneration(IEvolutionaryPopulation population)
        {
            Memory[0] = population.CompareCriteria.GetBetter(Memory[0], population.Best);
        }
    }
}
