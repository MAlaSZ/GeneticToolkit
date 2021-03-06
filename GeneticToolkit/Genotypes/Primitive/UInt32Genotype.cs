﻿using GeneticToolkit.Interfaces;
using System;
using JetBrains.Annotations;

namespace GeneticToolkit.Genotypes.Primitive
{
    [PublicAPI]
    public class UInt32Genotype : GenericPrimitiveGenotype<uint>
    {
        public UInt32Genotype() : base(sizeof(uint))
        {
            Genes = BitConverter.GetBytes(0);
        }

        public UInt32Genotype(uint value) : base(sizeof(uint))
        {
            Genes = BitConverter.GetBytes(value);
        }

        public override IGenotype ShallowCopy()
        {
            return new UInt32Genotype(sizeof(uint))
            {
                Genes = Genes.Clone() as byte[],
            };
        }

        public override IGenotype EmptyCopy()
        {
            return new UInt32Genotype(sizeof(uint));
        }

        public override T EmptyCopy<T>()
        {
            return (T) EmptyCopy();
        }
    }
}