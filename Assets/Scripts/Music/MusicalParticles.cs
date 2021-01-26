using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class MusicalParticles : MonoBehaviour
{
    public MusicManager manager;
    public bool auto;
    public bool reactToParticleCollisions;
    public int BPM = 120;
    public int audioSourceCap;
    [Range(0, 1)]
    public float playChance;
    [Range(0, 1)]
    public float volume;
    public float volumeMultiplier;
    //Relative: Quarter note is played one quarter note's length after the last note played.
    //Absolute: Quarter note is played on the metronomic quarter note position of the tempo. I.E. relative, if all the notes that came before it were quarter notes
    public bool relativeBeatSteps;
    private WeightSet<bool> playWeights;
    public string[] velNotesFilepaths;
    public int[] velWeights;

    [Range(0, 25)]
    public int wholeNoteWeight;

    [Range(0, 25)]
    public int halfNoteWeight;

    [Range(0, 25)]
    public int quarterNoteWeight;

    [Range(0, 25)]
    public int eigthNoteWeight;

    [Range(0, 25)]
    public int sixteenthNoteWeight;

    [Range(0, 25)]
    public int thirtySecondthNoteWeight;

    public float lastStep;
    public int lastNote = -1;
    public int notesQueued = 1;
    public Beat nextStep;
    public int nextVel;
    public ArrayList noteFiles;
    private AudioSource src;
    public string audioPathPrefix;

    private Queue<int> intervalQueue;

    public enum Beat : ushort
    {
        WHOLE = 1,
        HALF = 2,
        QUARTER = 4,
        EIGTH = 8,

        SIXTEENTH = 16,

        THIRYSECOND = 32
    }

    WeightSet<Beat> beatWeights1;
    WeightSet<Beat> beatWeights2;
    WeightSet<Beat> sliderBeatWeights;
    WeightSet<int> intervalWeights1;
    WeightSet<int> intervalWeights2;
    WeightSet<int[]> intervalWeights3;
    WeightSet<int> noteVelWeights1;
    WeightSet<float> panWeights;

    static T RandomEnumValue<T>()
    {
        var v = Enum.GetValues(typeof(T));
        return (T)v.GetValue((int)Mathf.Floor(UnityEngine.Random.Range(0, v.Length)));
    }
    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;
    // Start is called before the first frame update
    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
        src = GetComponent<AudioSource>();
        noteFiles = new ArrayList();

        audioPathPrefix = Path.Combine("Audio", "PartNotes");

        for (int n = 0; n < velNotesFilepaths.Length; n++)
        {
            string[] prefixes = velNotesFilepaths[n].Split('%');
            string[] suffixes = prefixes[1].Split('+');
            foreach (var suffix in suffixes)
            {
                addAudio(n, prefixes[0], suffix);
                Debug.Log("Loaded " + prefixes[0] + suffix);
            }
        }

        beatWeights1 = new WeightSet<Beat>();
        beatWeights1.addWeight(Beat.WHOLE, 1);
        beatWeights1.addWeight(Beat.HALF, 2);
        beatWeights1.addWeight(Beat.QUARTER, 4);
        beatWeights1.addWeight(Beat.EIGTH, 2);
        beatWeights1.addWeight(Beat.SIXTEENTH, 8);

        beatWeights2 = new WeightSet<Beat>();
        beatWeights2.addWeight(Beat.QUARTER, 1);
        beatWeights2.addWeight(Beat.EIGTH, 2);
        beatWeights2.addWeight(Beat.SIXTEENTH, 2);

        sliderBeatWeights = new WeightSet<Beat>();
        sliderBeatWeights.addWeight(Beat.WHOLE, wholeNoteWeight);
        sliderBeatWeights.addWeight(Beat.HALF, halfNoteWeight);
        sliderBeatWeights.addWeight(Beat.QUARTER, quarterNoteWeight);
        sliderBeatWeights.addWeight(Beat.EIGTH, eigthNoteWeight);
        sliderBeatWeights.addWeight(Beat.SIXTEENTH, sixteenthNoteWeight);
        sliderBeatWeights.addWeight(Beat.THIRYSECOND, thirtySecondthNoteWeight);

        intervalWeights1 = new WeightSet<int>();
        //Third
        intervalWeights1.addWeight(2, 8);
        intervalWeights1.addWeight(-2, 8);
        //Fifth       
        intervalWeights1.addWeight(4, 3);
        intervalWeights1.addWeight(-4, 3);
        //Seventh
        intervalWeights1.addWeight(6, 2);
        intervalWeights1.addWeight(-6, 2);

        //Same
        intervalWeights1.addWeight(0, 2);
        //Second
        intervalWeights1.addWeight(1, 1);
        intervalWeights1.addWeight(-1, 1);
        //Sixth
        intervalWeights1.addWeight(-5, 1);
        intervalWeights1.addWeight(5, 1);


        intervalWeights2 = new WeightSet<int>();
        //Same
        intervalWeights2.addWeight(0, 2);
        //Third
        intervalWeights2.addWeight(2, 1);
        intervalWeights2.addWeight(-2, 1);
        //Fifth
        intervalWeights2.addWeight(-4, 2);
        intervalWeights2.addWeight(4, 2);


        intervalWeights3 = new WeightSet<int[]>();

        //Same
        intervalWeights3.addWeight(new int[] { 0 }, 2);
        //Third
        intervalWeights3.addWeight(new int[] { 2 }, 1);
        intervalWeights3.addWeight(new int[] { -2 }, 1);

        //Triads
        intervalWeights3.addWeight(new int[] { 2, 2 }, 1);
        intervalWeights3.addWeight(new int[] { -2, -2 }, 1);
        intervalWeights3.addWeight(new int[] { -2, 4 }, 1);

        //Seventh
        intervalWeights3.addWeight(new int[] { 2, 4 }, 1);

        //Octave
        //intervalWeights3.addWeight(new int[] { -7, 7 }, 1);

        //Other
        intervalWeights3.addWeight(new int[] { 2, 2, -2, -2, -2, 2 }, 1);
        intervalWeights3.addWeight(new int[] { 2, 2, 2, 2, 2, 2 }, 1); //Dec 30
        intervalWeights3.addWeight(new int[] { -2, 2, 2, 1, 2, 2, -3, 2, 2 }, 1); //Dec 30
        intervalWeights3.addWeight(new int[] { 1, -1, 2, -1, 2, -1, 2}, 1); //Dec 30
        intervalWeights3.addWeight(new int[] { 4, 2, -6, 4, 2}, 1); //Dec 30
        intervalWeights3.addWeight(new int[] { 2, 2, -2 }, 1);
        intervalWeights3.addWeight(new int[] { 0, 0, 5, -1, -2, -2, 0 }, 1);

        panWeights = new WeightSet<float>();
        panWeights.addWeight(-0.5F, 1);
        panWeights.addWeight(0.5F, 1);
        panWeights.addWeight(-0.25F, 2);
        panWeights.addWeight(0.25F, 2);
        panWeights.addWeight(-0.1F, 4);
        panWeights.addWeight(0.1F, 4);

        noteVelWeights1 = new WeightSet<int>();
        for (int v = 0; v < velWeights.Length; v++)
        {
            noteVelWeights1.addWeight(v, velWeights[v]);
        }

        playWeights = new WeightSet<bool>();
        playWeights.addWeight(true, Mathf.FloorToInt(playChance * 100));
        playWeights.addWeight(false, 100 - Mathf.FloorToInt(playChance * 100));

        intervalQueue = new Queue<int>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((notesQueued > 0 || auto) && isNextStep(nextStep))
        {
            //If two particles hit before their note is played, they play on the same step.
            //Do-while to allow auto to run properly.
            notesQueued += auto ? 1 : 0;

            do
            {
                if (manager != null) {
                    if (lastNote == -1)
                        playNote(nextVel, getRandomNote(), playWeights);
                    else if (notesQueued <= 1) playNote(nextVel, getInterval(nextVel, manager.lastNote, manager.getNextInterval(intervalWeights3)), playWeights); //On one offs, pick a random
                    else playNote(nextVel, getInterval(nextVel, manager.lastNote, manager.getNextInterval(intervalWeights3)), playWeights); //Try to use a nicer interval weight set with more than one
                }
                else
                {
                    if (lastNote == -1)
                        playNote(nextVel, getRandomNote(), playWeights);
                    else if (notesQueued <= 1) playNote(nextVel, getInterval(nextVel, lastNote, getNextInterval()), playWeights); //On one offs, pick a random
                    else playNote(nextVel, getInterval(nextVel, lastNote, getNextInterval(intervalWeights3)), playWeights); //Try to use a nicer interval weight set with more than one
                }
            } while (notesQueued > 0);
        }

        int length = gameObject.GetComponents<AudioSource>().Length;
        AudioSource[] srcs = gameObject.GetComponents<AudioSource>();
        //Debug.Log(length);
        for (int s = 0; s < length; s++)
        {
            AudioSource src = srcs[s];
            try
            {
                if (src.time >= src.clip.length)
                {
                    DestroyImmediate(src);
                }
            }
            catch (Exception e) { };

            try
            {
                if (src.clip == null)
                {
                    if (src != null)
                    {
                        DestroyImmediate(src);
                    }

                }

                if (src != null)
                {
                    if (!src.isPlaying)
                    {
                        DestroyImmediate(src);
                    }
                }
            }
            catch (Exception e) { /*Debug.Log(e);*/ };
        }
    }

    void OnParticleCollision(GameObject other)
    {
        if (reactToParticleCollisions)
        {
            try
            {
                int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

                Rigidbody rb = other.GetComponent<Rigidbody>();
                int i = 0;

                while (i < numCollisionEvents)
                {
                    if (rb)
                    {
                        Vector3 pos = collisionEvents[i].intersection;
                        Vector3 force = (collisionEvents[i].intersection - transform.position) * collisionEvents[i].velocity.magnitude * 10;
                        rb.AddForce(force);
                    }
                    interact();
                    i++;
                }
            }
            catch (Exception e) { }
        }
    }

    public void interact()
    {
        notesQueued++;
    }

    void addAudio(int vel, string filePrefix, string fileSuffix)
    {
        string[] notes = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        foreach (var note in notes)
        {
            StartCoroutine(tryAddAudioFile(vel, Path.Combine(Application.streamingAssetsPath, audioPathPrefix, filePrefix + note + fileSuffix + ".wav")));
        }
    }

    IEnumerator tryAddAudioFile(int vel, string filepath)
    {
        while (noteFiles.Count <= vel)
            noteFiles.Add(new ArrayList());

        string uri = (new Uri(filepath)).AbsoluteUri;

        if (Application.platform == RuntimePlatform.Android)
            uri = (new Uri(filepath.Replace("#", "%23"))).AbsoluteUri;

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.responseCode != 200)
            {
                Debug.Log(uri + " | " + www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                ((ArrayList)noteFiles[vel]).Add(clip);
                //Debug.Log("Successfully loaded " + filepath);
            }
        }
    }

    Beat getNextBeat()
    {
        return sliderBeatWeights.getRandomValue();
    }

    int getNextVel()
    {
        return noteVelWeights1.getRandomValue();
    }

    int getNextInterval()
    {
        return getNextInterval(intervalWeights1);
    }

    int getNextInterval(WeightSet<int> weightSet)
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

    int getNextInterval(WeightSet<int[]> weightSet)
    {
        if (intervalQueue.Count == 0)
        {
            int[] intervalSequence = weightSet.getRandomValue();
            Debug.Log("Enqueueing seq " + intArrayToString(intervalSequence) + ".");
            foreach (var interval in intervalSequence)
            {
                intervalQueue.Enqueue(interval);
            }
        }
        //Debug.Log("Dequeuing " + intervalQueue.Peek());
        return intervalQueue.Dequeue();
    }

    public static string intArrayToString(int[] arr)
    {
        string s = "";
        for (int i = 0; i < arr.Length; i++)
        {
            s += arr[i];
            if (i != arr.Length - 1)
                s += ',';
        }
        return s;
    }

    bool isNextStep(Beat beat)
    {
        float beatLength = getBeatLength(beat);
        float nextBeatTimestamp = (lastStep - (lastStep % beatLength)) + beatLength;
        if (relativeBeatSteps)
        {
            if (Time.time - lastStep >= beatLength)
            {
                lastStep = Time.time;
                return true;
            }
        }
        else
        {
            if (Time.time >= nextBeatTimestamp)
            {
                lastStep = Time.time;
                return true;
            }
        }
        return false;
    }

    void playNote(int vel, int index, WeightSet<bool> playWeights)
    {
        if (gameObject.GetComponents<AudioSource>().Length < audioSourceCap && playWeights.getRandomValue())
        {
            AudioSource newSrc = gameObject.AddComponent<AudioSource>();
            newSrc.panStereo = panWeights.getRandomValue() * UnityEngine.Random.Range(0F, 1F);
            newSrc.volume = volume * volumeMultiplier;
            newSrc.clip = getAudioClip(vel, index);
            newSrc.Play();
        }
        lastNote = index;
        if(manager != null)
            manager.lastNote = index;
        nextStep = getNextBeat();
        nextVel = getNextVel();
        notesQueued--;
    }

    AudioClip getAudioClip(int vel, int index)
    {
        //Debug.Log("Getting note (" + vel + ", " + index + ")");
        return (AudioClip)(((ArrayList)noteFiles[vel])[index]);
    }

    int getRandomNote()
    {
        return (int)Mathf.Floor(UnityEngine.Random.Range(0, ((ArrayList)noteFiles[0]).Count));
    }

    int getRandomNote(int rangeStart, int rangeLength)
    {
        return (int)Mathf.Floor(UnityEngine.Random.Range(rangeStart, rangeStart + rangeLength));
    }

    int getInterval(int vel, int lastNote, int interval)
    {
        int notesCount = ((ArrayList)noteFiles[vel]).Count;

        int newNote = lastNote + interval;
        if (newNote < 0)
            newNote = (notesCount - (Mathf.Abs(newNote) % notesCount)) % notesCount;
        else newNote = newNote % notesCount;
        //Debug.Log("Note Index: " + newNote + " (" + notesCount + ") (" + lastNote + ", " + interval + ")");
        return newNote;
    }

    int getIntervalInRange(int vel, int lastNote, int interval, int rangeStart, int rangeLength)
    {
        int newNote = getInterval(vel, lastNote, interval);

        newNote = (newNote - rangeStart) % rangeLength;
        newNote += rangeStart;
        return newNote;
    }

    float getBeatLength(Beat beat)
    {
        return (60F / BPM) * 4 / (float)beat;
    }
}
