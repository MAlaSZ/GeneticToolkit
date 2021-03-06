﻿using JetBrains.Annotations;

namespace GeneticToolkit.Interfaces
{
    [PublicAPI]
    public interface IIndividual
    {
        IGenotype Genotype { get; set; }

        IPhenotype Phenotype { get; }

        double Value { get; }
        int CompareTo(IIndividual other, ICompareCriteria criteria);
    }
}
