﻿using GeneticToolkit.Genotypes.Collective.Combinatory;
using JetBrains.Annotations;

namespace GeneticToolkit.Crossovers
{
    [PublicAPI]
    public class AlternatingEdgeCrossover : SubTourChunkCrossover
    {
        protected override short[] GetChildValues(AdjacencyListGenotype[] parents, int genotypeSize, int startingParent)
        {
            InitializeVariables(parents, genotypeSize, startingParent);

            for (var i = 0; i < genotypeSize - 1; i++)
            {
                // 1. Select current parent
                ParentIndex = ++SubTourIndex % ParentsCount;

                // 2. Insert target - already assured that is OK
                ChildValues[StartIndex] = GetTarget(parents);
            }

            return ChildValues;
        }
    }
}