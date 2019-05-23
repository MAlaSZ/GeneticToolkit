﻿using GeneticToolkit.Interfaces;

using System;

namespace GeneticToolkit.Genotypes.Primitive
{
    public class Float32Genotype : GenericPrimitiveGenotype<float>
    {
        public Float32Genotype(int size) : base(size) { }

        public Float32Genotype(byte[] bytes) : base(bytes) { }

        public Float32Genotype(float value) : base(sizeof(float))
        {
            Genes = BitConverter.GetBytes(value);
        }

        public override IGenotype ShallowCopy()
        {
            return new Float32Genotype(sizeof(float))
            {
                Genes = this.Genes.Clone() as byte[],
            };
        }

        public override IGenotype EmptyCopy()
        {
            return new Float32Genotype(sizeof(float));
        }

        public override T EmptyCopy<T>()
        {
            return (T) EmptyCopy();
        }
    }
}
