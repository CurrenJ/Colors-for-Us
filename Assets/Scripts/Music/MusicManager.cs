using UnityEngine;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour {
     
    Queue<int> intervalQueue;
    public int lastNote;

    public MusicManager(){
        intervalQueue = new Queue<int>();
    }

    public int getNextInterval(WeightSet<int> weightSet)
    {
        if (intervalQueue.Count == 0)
        {
            int interval = weightSet.getRandomValue();
            //Debug.Log("Enqueueing " + interval + " and dequeuing to play.");
            intervalQueue.Enqueue(interval);
        }
        //else Debug.Log("Dequeueing " + intervalQueue.Peek());
        return intervalQueue.Dequeue();
    }

    public int getNextInterval(WeightSet<int[]> weightSet)
    {
        if (intervalQueue.Count == 0)
        {
            int[] intervalSequence = weightSet.getRandomValue();
            Debug.Log("Enqueueing seq " + MusicalParticles.intArrayToString(intervalSequence) + ".");
            foreach (var interval in intervalSequence)
            {
                intervalQueue.Enqueue(interval);
            }
        }
        //Debug.Log("Dequeuing " + intervalQueue.Peek());
        return intervalQueue.Dequeue();
    }
}