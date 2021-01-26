using System.Collections;
using System.Collections.Generic;
using System;

public class WeightSet<T1>
    {
        ArrayList weights;
        int totalWeight;
        public WeightSet()
        {
            totalWeight = 0;
            weights = new ArrayList();
        }

        public void addWeight(T1 t1, int weight)
        {
            weights.Add(new Tuple<int, T1>(weight, t1));
            totalWeight += weight;
        }

        public T1 getRandomValue()
        {
            float seed = UnityEngine.Random.Range(0, totalWeight);
            int temp = 0;
            for (int w = 0; w < weights.Count; w++)
            {
                Tuple<int, T1> tup = (Tuple<int, T1>)(weights[w]);
                if (seed >= temp && seed < tup.Item1 + temp)
                    return tup.Item2;
                temp += tup.Item1;
            }
            return ((Tuple<int, T1>)weights[0]).Item2;
        }
    }