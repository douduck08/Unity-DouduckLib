using UnityEngine;
using System.Collections.Generic;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

// /////////////////////////////////////////////////////////////////////////////////////////
//                              More Effective Coroutines
//                                        v2.02.1
// 
// This is an improved implementation of coroutines that boasts zero per-frame memory allocations,
// runs about twice as fast as Unity's built in coroutines and has a range of extra features.
// 
// This is the free version. MEC also has a pro version, which can be found here:
// https://www.assetstore.unity3d.com/en/#!/content/68480
// The pro version contains exactly the same core that the free version uses, but also
// contains additional features.
// 
// For manual, support, or upgrade guide visit http://trinary.tech/
//
// Created by Teal Rogers
// Trinary Software
// All rights preserved
// /////////////////////////////////////////////////////////////////////////////////////////

namespace MovementEffects
{
    public class Timing : MonoBehaviour
    {
        public enum DebugInfoType
        {
            None,
            SeperateCoroutines,
            SeperateTags
        }

        /// <summary>
        /// The time between calls to SlowUpdate.
        /// </summary>
        [Tooltip("How quickly the SlowUpdate segment ticks.")]
        public float TimeBetweenSlowUpdateCalls = 1f / 7f;
        /// <summary>
        /// The amount that each coroutine should be seperated inside the Unity profiler. NOTE: When the profiler window
        /// is not open this value is ignored and all coroutines behave as if "None" is selected.
        /// </summary>
        [Tooltip("How much data should be sent to the profiler window when it's open.")]
        public DebugInfoType ProfilerDebugAmount = DebugInfoType.SeperateCoroutines;
        /// <summary>
        /// Whether the manual timeframe should automatically trigger during the update segment.
        /// </summary>
        [Tooltip("When using manual timeframe, should it run automatically after the update loop or only when TriggerManualTimframeUpdate is called.")]
        public bool AutoTriggerManualTimeframe = true;
        /// <summary>
        /// The number of coroutines that are being run in the Update segment.
        /// </summary>
        [Tooltip("A count of the number of Update coroutines that are currently running."), Space(12)]
        public int UpdateCoroutines;
        /// <summary>
        /// The number of coroutines that are being run in the FixedUpdate segment.
        /// </summary>
        [Tooltip("A count of the number of FixedUpdate coroutines that are currently running.")]
        public int FixedUpdateCoroutines;
        /// <summary>
        /// The number of coroutines that are being run in the LateUpdate segment.
        /// </summary>
        [Tooltip("A count of the number of LateUpdate coroutines that are currently running.")]
        public int LateUpdateCoroutines;
        /// <summary>
        /// The number of coroutines that are being run in the SlowUpdate segment.
        /// </summary>
        [Tooltip("A count of the number of SlowUpdate coroutines that are currently running.")]
        public int SlowUpdateCoroutines;

        [HideInInspector]
        public double localTime;
        public static float LocalTime { get { return (float)Instance.localTime; } }
        [HideInInspector]
        public float deltaTime;
        public static float DeltaTime { get { return Instance.deltaTime; } }

        private bool _runningUpdate;
        private bool _runningLateUpdate;
        private bool _runningFixedUpdate;
        private int _nextUpdateProcessSlot;
        private int _nextLateUpdateProcessSlot;
        private int _nextFixedUpdateProcessSlot;
        private int _nextSlowUpdateProcessSlot;
        private double _lastUpdateTime;
        private double _lastLateUpdateTime;
        private double _lastFixedUpdateTime;
        private double _lastSlowUpdateTime;
        private ushort _framesSinceUpdate;
        private ushort _expansions = 1;

        private const ushort FramesUntilMaintenance = 64;
        private const int ProcessArrayChunkSize = 64;
        private const int InitialBufferSizeLarge = 256;
        private const int InitialBufferSizeMedium = 64;
        private const int InitialBufferSizeSmall = 8;

        /// <summary>
        /// When defined, all errors from inside coroutines will be passed into this function instead of falling through to the Unity console.
        /// </summary>
        public System.Action<System.Exception> OnError;
        /// <summary>
        /// Used for advanced coroutine control.
        /// </summary>
        public static System.Func<IEnumerator<float>, Timing, CoroutineHandle, IEnumerator<float>> ReplacementFunction;

        private readonly Dictionary<CoroutineHandle, HashSet<ProcessData>> _waitingTriggers = new Dictionary<CoroutineHandle, HashSet<ProcessData>>();
        private readonly Queue<System.Exception> _exceptions = new Queue<System.Exception>();
        private readonly Dictionary<CoroutineHandle, ProcessIndex> _handleToIndex = new Dictionary<CoroutineHandle, ProcessIndex>();
        private readonly Dictionary<ProcessIndex, CoroutineHandle> _indexToHandle = new Dictionary<ProcessIndex, CoroutineHandle>();
        private readonly Dictionary<ProcessIndex, string> _processTags = new Dictionary<ProcessIndex, string>();
        private readonly Dictionary<string, HashSet<ProcessIndex>> _taggedProcesses = new Dictionary<string, HashSet<ProcessIndex>>();

        private IEnumerator<float>[] UpdateProcesses = new IEnumerator<float>[InitialBufferSizeLarge];
        private IEnumerator<float>[] LateUpdateProcesses = new IEnumerator<float>[InitialBufferSizeSmall];
        private IEnumerator<float>[] FixedUpdateProcesses = new IEnumerator<float>[InitialBufferSizeMedium];
        private IEnumerator<float>[] SlowUpdateProcesses = new IEnumerator<float>[InitialBufferSizeMedium];

        private volatile bool[] UpdatePaused = new bool[InitialBufferSizeLarge];
        private volatile bool[] LateUpdatePaused = new bool[InitialBufferSizeSmall];
        private volatile bool[] FixedUpdatePaused = new bool[InitialBufferSizeMedium];
        private volatile bool[] SlowUpdatePaused = new bool[InitialBufferSizeMedium];

        private static Timing _instance;
        public static Timing Instance
        {
            get
            {
                if (_instance == null || !_instance.gameObject)
                {
                    GameObject instanceHome = GameObject.Find("Movement Effects");
                    System.Type movementType =
                        System.Type.GetType("MovementEffects.Movement, MovementOverTime, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

                    if(instanceHome == null)
                    {
                        instanceHome = new GameObject { name = "Movement Effects" };
                        DontDestroyOnLoad(instanceHome);

                        if (movementType != null)
                            instanceHome.AddComponent(movementType);

                        _instance = instanceHome.AddComponent<Timing>();
                    }
                    else
                    {
                         if (movementType != null && instanceHome.GetComponent(movementType) == null) 
                            instanceHome.AddComponent(movementType);

                        _instance = instanceHome.GetComponent<Timing>() ?? instanceHome.AddComponent<Timing>();
                    }
                }

                return _instance;
            }

            set { _instance = value; }
        }

        void Awake()
        {
            if(_instance == null)
                _instance = this;
            else
                deltaTime = _instance.deltaTime;
        }

        void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        void Update()
        {
            if(_lastSlowUpdateTime + TimeBetweenSlowUpdateCalls < Time.realtimeSinceStartup && _nextSlowUpdateProcessSlot > 0)
            {
                ProcessIndex coindex = new ProcessIndex { seg = Segment.SlowUpdate };
                UpdateTimeValues(coindex.seg);

                for (coindex.i = 0; coindex.i < _nextSlowUpdateProcessSlot; coindex.i++)
                {
                    if (!SlowUpdatePaused[coindex.i] && SlowUpdateProcesses[coindex.i] != null && !(localTime < SlowUpdateProcesses[coindex.i].Current))
                    {
                        if (ProfilerDebugAmount != DebugInfoType.None)
                        {
                            Profiler.BeginSample(ProfilerDebugAmount == DebugInfoType.SeperateTags
                                                     ? ("Processing Coroutine (Slow Update)" +
                                                        (_processTags.ContainsKey(coindex) ? ", tag " + _processTags[coindex] : ", no tag"))
                                                     : "Processing Coroutine (Slow Update)");
                        }

                        try
                        {
                            if (!SlowUpdateProcesses[coindex.i].MoveNext())
                            {
                                SlowUpdateProcesses[coindex.i] = null;
                            }
                            else if (SlowUpdateProcesses[coindex.i] != null && float.IsNaN(SlowUpdateProcesses[coindex.i].Current))
                            {
                                if(ReplacementFunction == null)
                                {
                                    SlowUpdateProcesses[coindex.i] = null;
                                }
                                else
                                {
                                    SlowUpdateProcesses[coindex.i] = ReplacementFunction(SlowUpdateProcesses[coindex.i], this, _indexToHandle[coindex]);

                                    ReplacementFunction = null;
                                    coindex.i--;
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            if (OnError == null)
                                _exceptions.Enqueue(ex);
                            else
                                OnError(ex);

                            SlowUpdateProcesses[coindex.i] = null;
                        }

                        if (ProfilerDebugAmount != DebugInfoType.None)
                            Profiler.EndSample();
                    }
                }
            }

            if (_nextUpdateProcessSlot > 0)
            {
                ProcessIndex coindex = new ProcessIndex { seg = Segment.Update };
                _runningUpdate = true;
                UpdateTimeValues(coindex.seg);

                for (coindex.i = 0; coindex.i < _nextUpdateProcessSlot; coindex.i++)
                {
                    if (!UpdatePaused[coindex.i] && UpdateProcesses[coindex.i] != null && !(localTime < UpdateProcesses[coindex.i].Current))
                    {
                        if (ProfilerDebugAmount != DebugInfoType.None)
                        {
                            Profiler.BeginSample(ProfilerDebugAmount == DebugInfoType.SeperateTags
                                                     ? ("Processing Coroutine" +
                                                        (_processTags.ContainsKey(coindex) ? ", tag " + _processTags[coindex] : ", no tag"))
                                                     : "Processing Coroutine");
                        }

                        try
                        {
                            if (!UpdateProcesses[coindex.i].MoveNext())
                            {
                                UpdateProcesses[coindex.i] = null;
                            }
                            else if (UpdateProcesses[coindex.i] != null && float.IsNaN(UpdateProcesses[coindex.i].Current))
                            {
                                if(ReplacementFunction == null)
                                {
                                    UpdateProcesses[coindex.i] = null;
                                }
                                else
                                {
                                    UpdateProcesses[coindex.i] = ReplacementFunction(UpdateProcesses[coindex.i], this, _indexToHandle[coindex]);

                                    ReplacementFunction = null;
                                    coindex.i--;
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            if (OnError == null)
                                _exceptions.Enqueue(ex);
                            else
                                OnError(ex);

                            UpdateProcesses[coindex.i] = null;
                        }

                        if (ProfilerDebugAmount != DebugInfoType.None)
                            Profiler.EndSample();
                    }
                }

                _runningUpdate = false;
            }

            if(++_framesSinceUpdate > FramesUntilMaintenance)
            {
                _framesSinceUpdate = 0;

                if (ProfilerDebugAmount != DebugInfoType.None)
                    Profiler.BeginSample("Maintenance Task");

                RemoveUnused();

                if (ProfilerDebugAmount != DebugInfoType.None)
                    Profiler.EndSample();
            }

            if (_exceptions.Count > 0)
                 throw _exceptions.Dequeue();
        }

        void FixedUpdate()
        {
            if(_nextFixedUpdateProcessSlot > 0)
            {
                ProcessIndex coindex = new ProcessIndex { seg = Segment.FixedUpdate };
                _runningFixedUpdate = true;
                UpdateTimeValues(coindex.seg);

                for (coindex.i = 0; coindex.i < _nextFixedUpdateProcessSlot; coindex.i++)
                {
                    if (!FixedUpdatePaused[coindex.i] && FixedUpdateProcesses[coindex.i] != null && !(localTime < FixedUpdateProcesses[coindex.i].Current))
                    {
                        if (ProfilerDebugAmount != DebugInfoType.None)
                        {
                            Profiler.BeginSample(ProfilerDebugAmount == DebugInfoType.SeperateTags
                                                     ? ("Processing Coroutine" +
                                                        (_processTags.ContainsKey(coindex) ? ", tag " + _processTags[coindex] : ", no tag"))
                                                     : "Processing Coroutine");
                        }

                        try
                        {
                            if (!FixedUpdateProcesses[coindex.i].MoveNext())
                            {
                                FixedUpdateProcesses[coindex.i] = null;
                            }
                            else if (FixedUpdateProcesses[coindex.i] != null && float.IsNaN(FixedUpdateProcesses[coindex.i].Current))
                            {
                                if(ReplacementFunction == null)
                                {
                                    FixedUpdateProcesses[coindex.i] = null;
                                }
                                else
                                {
                                    FixedUpdateProcesses[coindex.i] = ReplacementFunction(FixedUpdateProcesses[coindex.i], this, _indexToHandle[coindex]);

                                    ReplacementFunction = null;
                                    coindex.i--;
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            if (OnError == null)
                                _exceptions.Enqueue(ex);
                            else
                                OnError(ex);

                            FixedUpdateProcesses[coindex.i] = null;
                        }

                        if (ProfilerDebugAmount != DebugInfoType.None)
                            Profiler.EndSample();
                    }
                }

                _runningFixedUpdate = false;
            }

            if (_exceptions.Count > 0)
                throw _exceptions.Dequeue();
        }

        void LateUpdate()
        {
            if(_nextLateUpdateProcessSlot > 0)
            {
                ProcessIndex coindex = new ProcessIndex { seg = Segment.LateUpdate };
                _runningLateUpdate = true;
                UpdateTimeValues(coindex.seg);

                for (coindex.i = 0; coindex.i < _nextLateUpdateProcessSlot; coindex.i++)
                {
                    if (!LateUpdatePaused[coindex.i] && LateUpdateProcesses[coindex.i] != null && !(localTime < LateUpdateProcesses[coindex.i].Current))
                    {
                        if (ProfilerDebugAmount != DebugInfoType.None)
                        {
                            Profiler.BeginSample(ProfilerDebugAmount == DebugInfoType.SeperateTags
                                                     ? ("Processing Coroutine" +
                                                        (_processTags.ContainsKey(coindex) ? ", tag " + _processTags[coindex] : ", no tag"))
                                                     : "Processing Coroutine");
                        }

                        try
                        {
                            if (!LateUpdateProcesses[coindex.i].MoveNext())
                            {
                                LateUpdateProcesses[coindex.i] = null;
                            }
                            else if (LateUpdateProcesses[coindex.i] != null && float.IsNaN(LateUpdateProcesses[coindex.i].Current))
                            {
                                if(ReplacementFunction == null)
                                {
                                    LateUpdateProcesses[coindex.i] = null;
                                }
                                else
                                {
                                    LateUpdateProcesses[coindex.i] = ReplacementFunction(LateUpdateProcesses[coindex.i], this, _indexToHandle[coindex]);

                                    ReplacementFunction = null;
                                    coindex.i--;
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            if (OnError == null)
                                _exceptions.Enqueue(ex);
                            else
                                OnError(ex);

                            LateUpdateProcesses[coindex.i] = null;
                        }

                        if (ProfilerDebugAmount != DebugInfoType.None)
                            Profiler.EndSample();
                    }
                }

                _runningLateUpdate = false;
            }

            if (_exceptions.Count > 0)
                throw _exceptions.Dequeue();
        }

        private void RemoveUnused()
        {
            var waitTrigsEnum = _waitingTriggers.GetEnumerator();
            while (waitTrigsEnum.MoveNext())
            {
                if (waitTrigsEnum.Current.Value.Count == 0)
                {
                    _waitingTriggers.Remove(waitTrigsEnum.Current.Key);
                    waitTrigsEnum = _waitingTriggers.GetEnumerator();
                    continue;
                }

                if (_handleToIndex.ContainsKey(waitTrigsEnum.Current.Key) && CoindexIsNull(_handleToIndex[waitTrigsEnum.Current.Key]))
                {
                    CloseWaitingProcess(waitTrigsEnum.Current.Key);
                    waitTrigsEnum = _waitingTriggers.GetEnumerator();
                }
            }

            ProcessIndex outer, inner;
            outer.seg = inner.seg = Segment.Update;
            for (outer.i = inner.i = 0; outer.i < _nextUpdateProcessSlot; outer.i++)
            {
                if (UpdateProcesses[outer.i] != null)
                {
                    if (outer.i != inner.i)
                    {
                        UpdateProcesses[inner.i] = UpdateProcesses[outer.i];
                        UpdatePaused[inner.i] = UpdatePaused[outer.i];
                        MoveTag(outer, inner);

                        if (_indexToHandle.ContainsKey(inner))
                        {
                            _handleToIndex.Remove(_indexToHandle[inner]);
                            _indexToHandle.Remove(inner);
                        }

                        _handleToIndex[_indexToHandle[outer]] = inner;
                        _indexToHandle.Add(inner, _indexToHandle[outer]);
                        _indexToHandle.Remove(outer);
                    }
                    inner.i++;
                }
            }
            for (outer.i = inner.i; outer.i < _nextUpdateProcessSlot; outer.i++)
            {
                UpdateProcesses[outer.i] = null;
                UpdatePaused[outer.i] = false;
                RemoveTag(outer);

                if (_indexToHandle.ContainsKey(outer))
                {
                    _handleToIndex.Remove(_indexToHandle[outer]);
                    _indexToHandle.Remove(outer);
                }
            }

            UpdateCoroutines = _nextUpdateProcessSlot = inner.i;

            outer.seg = inner.seg = Segment.FixedUpdate;
            for (outer.i = inner.i = 0; outer.i < _nextFixedUpdateProcessSlot; outer.i++)
            {
                if (FixedUpdateProcesses[outer.i] != null)
                {
                    if (outer.i != inner.i)
                    {
                        FixedUpdateProcesses[inner.i] = FixedUpdateProcesses[outer.i];
                        FixedUpdatePaused[inner.i] = FixedUpdatePaused[outer.i];
                        MoveTag(outer, inner);

                        if (_indexToHandle.ContainsKey(inner))
                        {
                            _handleToIndex.Remove(_indexToHandle[inner]);
                            _indexToHandle.Remove(inner);
                        }

                        _handleToIndex[_indexToHandle[outer]] = inner;
                        _indexToHandle.Add(inner, _indexToHandle[outer]);
                        _indexToHandle.Remove(outer);
                    }
                    inner.i++;
                }
            }
            for (outer.i = inner.i; outer.i < _nextFixedUpdateProcessSlot; outer.i++)
            {
                FixedUpdateProcesses[outer.i] = null;
                FixedUpdatePaused[outer.i] = false;
                RemoveTag(outer);

                if (_indexToHandle.ContainsKey(outer))
                {
                    _handleToIndex.Remove(_indexToHandle[outer]);
                    _indexToHandle.Remove(outer);
                }
            }

            FixedUpdateCoroutines = _nextFixedUpdateProcessSlot = inner.i;

            outer.seg = inner.seg = Segment.LateUpdate;
            for (outer.i = inner.i = 0; outer.i < _nextLateUpdateProcessSlot; outer.i++)
            {
                if (LateUpdateProcesses[outer.i] != null)
                {
                    if (outer.i != inner.i)
                    {
                        LateUpdateProcesses[inner.i] = LateUpdateProcesses[outer.i];
                        LateUpdatePaused[inner.i] = LateUpdatePaused[outer.i];
                        MoveTag(outer, inner);

                        if (_indexToHandle.ContainsKey(inner))
                        {
                            _handleToIndex.Remove(_indexToHandle[inner]);
                            _indexToHandle.Remove(inner);
                        }

                        _handleToIndex[_indexToHandle[outer]] = inner;
                        _indexToHandle.Add(inner, _indexToHandle[outer]);
                        _indexToHandle.Remove(outer);
                    }
                    inner.i++;
                }
            }
            for (outer.i = inner.i; outer.i < _nextLateUpdateProcessSlot; outer.i++)
            {
                LateUpdateProcesses[outer.i] = null;
                LateUpdatePaused[outer.i] = false;
                RemoveTag(outer);

                if (_indexToHandle.ContainsKey(outer))
                {
                    _handleToIndex.Remove(_indexToHandle[outer]);
                    _indexToHandle.Remove(outer);
                }
            }

            LateUpdateCoroutines = _nextLateUpdateProcessSlot = inner.i;

            outer.seg = inner.seg = Segment.SlowUpdate;
            for (outer.i = inner.i = 0; outer.i < _nextSlowUpdateProcessSlot; outer.i++)
            {
                if (SlowUpdateProcesses[outer.i] != null)
                {
                    if (outer.i != inner.i)
                    {
                        SlowUpdateProcesses[inner.i] = SlowUpdateProcesses[outer.i];
                        SlowUpdatePaused[inner.i] = SlowUpdatePaused[outer.i];
                        MoveTag(outer, inner);

                        if (_indexToHandle.ContainsKey(inner))
                        {
                            _handleToIndex.Remove(_indexToHandle[inner]);
                            _indexToHandle.Remove(inner);
                        }

                        _handleToIndex[_indexToHandle[outer]] = inner;
                        _indexToHandle.Add(inner, _indexToHandle[outer]);
                        _indexToHandle.Remove(outer);
                    }
                    inner.i++;
                }
            }
            for (outer.i = inner.i; outer.i < _nextSlowUpdateProcessSlot; outer.i++)
            {
                SlowUpdateProcesses[outer.i] = null;
                SlowUpdatePaused[outer.i] = false;
                RemoveTag(outer);

                if (_indexToHandle.ContainsKey(outer))
                {
                    _handleToIndex.Remove(_indexToHandle[outer]);
                    _indexToHandle.Remove(outer);
                }
            }

            SlowUpdateCoroutines = _nextSlowUpdateProcessSlot = inner.i;
        }

        /// <summary>
        /// Run a new coroutine in the Update segment.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public static CoroutineHandle RunCoroutine(IEnumerator<float> coroutine)
        {
            return coroutine == null ? null : Instance.RunCoroutineInternal(coroutine, Segment.Update, null, null);
        }

        /// <summary>
        /// Run a new coroutine in the Update segment.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="tag">An optional tag to attach to the coroutine which can later be used for Kill operations.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public static CoroutineHandle RunCoroutine(IEnumerator<float> coroutine, string tag)
        {
            return coroutine == null ? null : Instance.RunCoroutineInternal(coroutine, Segment.Update, tag, null);
        }

        /// <summary>
        /// Run a new coroutine.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="timing">The segment that the coroutine should run in.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public static CoroutineHandle RunCoroutine(IEnumerator<float> coroutine, Segment timing)
        {
            return coroutine == null ? null : Instance.RunCoroutineInternal(coroutine, timing, null, null);
        }

        /// <summary>
        /// Run a new coroutine.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="timing">The segment that the coroutine should run in.</param>
        /// <param name="tag">An optional tag to attach to the coroutine which can later be used for Kill operations.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public static CoroutineHandle RunCoroutine(IEnumerator<float> coroutine, Segment timing, string tag)
        {
            return coroutine == null ? null : Instance.RunCoroutineInternal(coroutine, timing, tag, null);
        }

        /// <summary>
        /// Run a new coroutine on this Timing instance in the Update segment.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public CoroutineHandle RunCoroutineOnInstance(IEnumerator<float> coroutine)
        {
            return coroutine == null ? null : RunCoroutineInternal(coroutine, Segment.Update, null, null);
        }

        /// <summary>
        /// Run a new coroutine on this Timing instance in the Update segment.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="tag">An optional tag to attach to the coroutine which can later be used for Kill operations.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public CoroutineHandle RunCoroutineOnInstance(IEnumerator<float> coroutine, string tag)
        {
            return coroutine == null ? null : RunCoroutineInternal(coroutine, Segment.Update, tag, null);
        }

        /// <summary>
        /// Run a new coroutine on this Timing instance.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="timing">The segment that the coroutine should run in.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public CoroutineHandle RunCoroutineOnInstance(IEnumerator<float> coroutine, Segment timing)
        {
            return coroutine == null ? null : RunCoroutineInternal(coroutine, timing, null, null);
        }

        /// <summary>
        /// Run a new coroutine on this Timing instance.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="timing">The segment that the coroutine should run in.</param>
        /// <param name="tag">An optional tag to attach to the coroutine which can later be used for Kill operations.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public CoroutineHandle RunCoroutineOnInstance(IEnumerator<float> coroutine, Segment timing, string tag)
        {
            return coroutine == null ? null : RunCoroutineInternal(coroutine, timing, tag, null);
        }

        private CoroutineHandle RunCoroutineInternal(IEnumerator<float> coroutine, Segment timing, string tag, CoroutineHandle handle)
        {
            ProcessIndex slot = new ProcessIndex { seg = timing };
            bool prewarm;
            if (handle == null)
            {
                prewarm = true;
                handle = new CoroutineHandle();
            }
            else
            {
                prewarm = false;

                if (_handleToIndex.ContainsKey(handle))
                {
                    _indexToHandle.Remove(_handleToIndex[handle]);
                    _handleToIndex.Remove(handle);
                }
            }

            switch (timing)
            {
                case Segment.Update:

                    if (_nextUpdateProcessSlot >= UpdateProcesses.Length)
                    {
                        IEnumerator<float>[] oldProcArray = UpdateProcesses;
                        bool[] oldPausedArray = UpdatePaused;

                        UpdateProcesses = new IEnumerator<float>[UpdateProcesses.Length + (ProcessArrayChunkSize * _expansions++)];
                        UpdatePaused = new bool[UpdateProcesses.Length];

                        for (int i = 0; i < oldProcArray.Length; i++)
                        {
                            UpdateProcesses[i] = oldProcArray[i];
                            UpdatePaused[i] = oldPausedArray[i];
                        }
                    }

                    slot.i = _nextUpdateProcessSlot++;
                    UpdateProcesses[slot.i] = coroutine;

                    if (null != tag)
                        AddTag(tag, slot);

                    _indexToHandle.Add(slot, handle);
                    _handleToIndex.Add(handle, slot);

                    if (!_runningUpdate && prewarm)
                    {
                        try
                        {
                            _runningUpdate = true;
                            SetTimeValues(slot.seg);

                            if (!UpdateProcesses[slot.i].MoveNext())
                            {
                                UpdateProcesses[slot.i] = null;
                            }
                            else if (UpdateProcesses[slot.i] != null && float.IsNaN(UpdateProcesses[slot.i].Current))
                            {
                                if (ReplacementFunction == null)
                                {
                                    UpdateProcesses[slot.i] = null;
                                }
                                else
                                {
                                    UpdateProcesses[slot.i] = ReplacementFunction(UpdateProcesses[slot.i], this, _indexToHandle[slot]);

                                    ReplacementFunction = null;

                                    if (UpdateProcesses[slot.i] != null)
                                        UpdateProcesses[slot.i].MoveNext();
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            if (OnError == null)
                                _exceptions.Enqueue(ex);
                            else
                                OnError(ex);

                            UpdateProcesses[slot.i] = null;
                        }
                        finally
                        {
                            _runningUpdate = false;
                        }
                    }

                    return handle;

                case Segment.FixedUpdate:

                    if (_nextFixedUpdateProcessSlot >= FixedUpdateProcesses.Length)
                    {
                        IEnumerator<float>[] oldProcArray = FixedUpdateProcesses;
                        bool[] oldPausedArray = FixedUpdatePaused;

                        FixedUpdateProcesses = new IEnumerator<float>[FixedUpdateProcesses.Length + (ProcessArrayChunkSize * _expansions++)];
                        FixedUpdatePaused = new bool[FixedUpdateProcesses.Length];

                        for (int i = 0; i < oldProcArray.Length; i++)
                        {
                            FixedUpdateProcesses[i] = oldProcArray[i];
                            FixedUpdatePaused[i] = oldPausedArray[i];
                        }
                    }

                    slot.i = _nextFixedUpdateProcessSlot++;
                    FixedUpdateProcesses[slot.i] = coroutine;

                    if (null != tag)
                        AddTag(tag, slot);

                    _indexToHandle.Add(slot, handle);
                    _handleToIndex.Add(handle, slot);

                    if (!_runningFixedUpdate && prewarm)
                    {
                        try
                        {
                            _runningFixedUpdate = true;
                            SetTimeValues(slot.seg);

                            if (!FixedUpdateProcesses[slot.i].MoveNext())
                            {
                                FixedUpdateProcesses[slot.i] = null;
                            }
                            else if (FixedUpdateProcesses[slot.i] != null && float.IsNaN(FixedUpdateProcesses[slot.i].Current))
                            {
                                if (ReplacementFunction == null)
                                {
                                    FixedUpdateProcesses[slot.i] = null;
                                }
                                else
                                {
                                    FixedUpdateProcesses[slot.i] = ReplacementFunction(FixedUpdateProcesses[slot.i], this, _indexToHandle[slot]);

                                    ReplacementFunction = null;

                                    if (FixedUpdateProcesses[slot.i] != null)
                                        FixedUpdateProcesses[slot.i].MoveNext();
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            if (OnError == null)
                                _exceptions.Enqueue(ex);
                            else
                                OnError(ex);

                            FixedUpdateProcesses[slot.i] = null;
                        }
                        finally
                        {
                            _runningFixedUpdate = false;
                        }
                    }

                    return handle;

                case Segment.LateUpdate:

                    if (_nextLateUpdateProcessSlot >= LateUpdateProcesses.Length)
                    {
                        IEnumerator<float>[] oldProcArray = LateUpdateProcesses;
                        bool[] oldPausedArray = LateUpdatePaused;

                        LateUpdateProcesses = new IEnumerator<float>[LateUpdateProcesses.Length + (ProcessArrayChunkSize * _expansions++)];
                        LateUpdatePaused = new bool[LateUpdateProcesses.Length];

                        for (int i = 0; i < oldProcArray.Length; i++)
                        {
                            LateUpdateProcesses[i] = oldProcArray[i];
                            LateUpdatePaused[i] = oldPausedArray[i];
                        }
                    }

                    slot.i = _nextLateUpdateProcessSlot++;
                    LateUpdateProcesses[slot.i] = coroutine;

                    if (tag != null)
                        AddTag(tag, slot);

                    _indexToHandle.Add(slot, handle);
                    _handleToIndex.Add(handle, slot);

                    if (!_runningLateUpdate && prewarm)
                    {
                        try
                        {
                            _runningLateUpdate = true;
                            SetTimeValues(slot.seg);

                            if (!LateUpdateProcesses[slot.i].MoveNext())
                            {
                                LateUpdateProcesses[slot.i] = null;
                            }
                            else if (LateUpdateProcesses[slot.i] != null && float.IsNaN(LateUpdateProcesses[slot.i].Current))
                            {
                                if (ReplacementFunction == null)
                                {
                                    LateUpdateProcesses[slot.i] = null;
                                }
                                else
                                {
                                    LateUpdateProcesses[slot.i] = ReplacementFunction(LateUpdateProcesses[slot.i], this, _indexToHandle[slot]);

                                    ReplacementFunction = null;

                                    if (LateUpdateProcesses[slot.i] != null)
                                        LateUpdateProcesses[slot.i].MoveNext();
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            if (OnError == null)
                                _exceptions.Enqueue(ex);
                            else
                                OnError(ex);

                            LateUpdateProcesses[slot.i] = null;
                        }
                        finally
                        {
                            _runningLateUpdate = false;
                        }
                    }

                    return handle;

                case Segment.SlowUpdate:

                    if (_nextSlowUpdateProcessSlot >= SlowUpdateProcesses.Length)
                    {
                        IEnumerator<float>[] oldProcArray = SlowUpdateProcesses;
                        bool[] oldPausedArray = SlowUpdatePaused;

                        SlowUpdateProcesses = new IEnumerator<float>[SlowUpdateProcesses.Length + (ProcessArrayChunkSize * _expansions++)];
                        SlowUpdatePaused = new bool[SlowUpdateProcesses.Length];

                        for (int i = 0; i < oldProcArray.Length; i++)
                        {
                            SlowUpdateProcesses[i] = oldProcArray[i];
                            SlowUpdatePaused[i] = oldPausedArray[i];
                        }
                    }

                    slot.i = _nextSlowUpdateProcessSlot++;
                    SlowUpdateProcesses[slot.i] = coroutine;

                    if (tag != null)
                        AddTag(tag, slot);

                    _indexToHandle.Add(slot, handle);
                    _handleToIndex.Add(handle, slot);

                    return handle;

                default:
                    return null;
            }
        }

        private void UpdateTimeValues(Segment segment)
        {
            switch(segment)
            {
                case Segment.Update:
                    deltaTime = Time.deltaTime;
                    _lastUpdateTime += deltaTime;
                    localTime = _lastUpdateTime;
                    break;
                case Segment.LateUpdate:
                    deltaTime = Time.deltaTime;
                    _lastLateUpdateTime += deltaTime;
                    localTime = _lastLateUpdateTime;
                    break;
                case Segment.FixedUpdate:
                    deltaTime = Time.deltaTime;
                    _lastFixedUpdateTime += deltaTime;
                    localTime = _lastFixedUpdateTime;
                    break;
                case Segment.SlowUpdate:
                    if(_lastSlowUpdateTime == 0d)
                        deltaTime = TimeBetweenSlowUpdateCalls;
                    else
                        deltaTime = Time.realtimeSinceStartup - (float)_lastSlowUpdateTime;

                    localTime = _lastSlowUpdateTime = Time.realtimeSinceStartup;
                    break;
            }
        }

        private void SetTimeValues(Segment segment)
        {
            switch (segment)
            {
                case Segment.Update:
                    deltaTime = Time.deltaTime;
                    localTime = _lastUpdateTime;
                    break;
                case Segment.LateUpdate:
                    deltaTime = Time.deltaTime;
                    localTime = _lastLateUpdateTime;
                    break;
                case Segment.FixedUpdate:
                    deltaTime = Time.deltaTime;
                    localTime = _lastFixedUpdateTime;
                    break;
                case Segment.SlowUpdate:
                    deltaTime = Time.realtimeSinceStartup - (float)_lastSlowUpdateTime;
                    localTime = _lastSlowUpdateTime;
                    break;
            }
        }

        private double GetSegmentTime(Segment segment)
        {
            switch (segment)
            {
                case Segment.Update:
                    return _lastUpdateTime;
                case Segment.LateUpdate:
                    return _lastLateUpdateTime;
                case Segment.FixedUpdate:
                    return _lastFixedUpdateTime;
                case Segment.SlowUpdate:
                    return _lastSlowUpdateTime;
                default:
                    return 0d;
            }
        }

        /// <summary>
        /// Resets the value of LocalTime to zero (only for the Update, LateUpdate, and FixedUpdate segments).
        /// </summary>
        public void ResetTimeCountOnInstance()
        {
            localTime = 0d;

            _lastUpdateTime = 0d;
            _lastLateUpdateTime = 0d;
            _lastFixedUpdateTime = 0d;
        }

        /// <summary>
        /// This will pause all coroutines running on the current MEC instance until ResumeCoroutines is called.
        /// </summary>
        /// <returns>The number of coroutines that were paused.</returns>
        public static int PauseCoroutines()
        {
            return _instance == null ? 0 : _instance.PauseCoroutinesOnInstance();
        }

        /// <summary>
        /// This will pause all coroutines running on this MEC instance until ResumeCoroutinesOnInstance is called.
        /// </summary>
        /// <returns>The number of coroutines that were paused.</returns>
        public int PauseCoroutinesOnInstance()
        {
            int count = 0;
            int i;
            for (i = 0; i < _nextUpdateProcessSlot; i++)
            {
                if (!UpdatePaused[i] && UpdateProcesses[i] != null)
                {
                    UpdatePaused[i] = true;
                    count++;
                }
            }

            for (i = 0; i < _nextLateUpdateProcessSlot; i++)
            {
                if (!LateUpdatePaused[i] && LateUpdateProcesses[i] != null)
                {
                    LateUpdatePaused[i] = true;
                    count++;
                }
            }

            for (i = 0; i < _nextFixedUpdateProcessSlot; i++)
            {
                if (!FixedUpdatePaused[i] && FixedUpdateProcesses[i] != null)
                {
                    FixedUpdatePaused[i] = true;
                    count++;
                }
            }

            for (i = 0; i < _nextSlowUpdateProcessSlot; i++)
            {
                if (!SlowUpdatePaused[i] && SlowUpdateProcesses[i] != null)
                {
                    SlowUpdatePaused[i] = true;
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// This will pause any matching coroutines running on the current MEC instance until ResumeCoroutines is called.
        /// </summary>
        /// <param name="tag">Any coroutines with a matching tag will be paused.</param>
        /// <returns>The number of coroutines that were paused.</returns>
        public static int PauseCoroutines(string tag)
        {
            return _instance == null ? 0 : _instance.PauseCoroutinesOnInstance(tag);
        }

        /// <summary>
        /// This will pause any matching coroutines running on this MEC instance until ResumeCoroutinesOnInstance is called.
        /// </summary>
        /// <param name="tag">Any coroutines with a matching tag will be paused.</param>
        /// <returns>The number of coroutines that were paused.</returns>
        public int PauseCoroutinesOnInstance(string tag)
        {
            if (tag == null || !_taggedProcesses.ContainsKey(tag))
                return 0;

            int count = 0;
            var matchesEnum = _taggedProcesses[tag].GetEnumerator();

            while (matchesEnum.MoveNext())
                if (!CoindexIsNull(matchesEnum.Current) && !CoindexSetPause(matchesEnum.Current))
                    count++;

            return count;
        }

        /// <summary>
        /// This resumes all coroutines on the current MEC instance if they are currently paused, otherwise it has
        /// no effect.
        /// </summary>
        /// <returns>The number of coroutines that were resumed.</returns>
        public static int ResumeCoroutines()
        {
            return _instance == null ? 0 : _instance.ResumeCoroutinesOnInstance();
        }

        /// <summary>
        /// This resumes all coroutines on this MEC instance if they are currently paused, otherwise it has no effect.
        /// </summary>
        /// <returns>The number of coroutines that were resumed.</returns>
        public int ResumeCoroutinesOnInstance()
        {
            int count = 0;
            int i;
            for (i = 0; i < _nextUpdateProcessSlot; i++)
            {
                if (UpdatePaused[i] && UpdateProcesses[i] != null)
                {
                    UpdatePaused[i] = false;
                    count++;
                }
            }

            for (i = 0; i < _nextLateUpdateProcessSlot; i++)
            {
                if (LateUpdatePaused[i] && LateUpdateProcesses[i] != null)
                {
                    LateUpdatePaused[i] = false;
                    count++;
                }
            }

            for (i = 0; i < _nextFixedUpdateProcessSlot; i++)
            {
                if (FixedUpdatePaused[i] && FixedUpdateProcesses[i] != null)
                {
                    FixedUpdatePaused[i] = false;
                    count++;
                }
            }

            for (i = 0; i < _nextSlowUpdateProcessSlot; i++)
            {
                if (SlowUpdatePaused[i] && SlowUpdateProcesses[i] != null)
                {
                    SlowUpdatePaused[i] = false;
                    count++;
                }
            }

            var waitingEnum = _waitingTriggers.GetEnumerator();
            while (waitingEnum.MoveNext())
            {
                int listCount = 0;
                var pausedList = waitingEnum.Current.Value.GetEnumerator();

                while (pausedList.MoveNext())
                {
                    if (_handleToIndex.ContainsKey(pausedList.Current.Handle) && !CoindexIsNull(_handleToIndex[pausedList.Current.Handle]))
                    {
                        CoindexSetPause(_handleToIndex[pausedList.Current.Handle]);
                        listCount++;
                    }
                    else
                    {
                        waitingEnum.Current.Value.Remove(pausedList.Current);
                        listCount = 0;
                        pausedList = waitingEnum.Current.Value.GetEnumerator();
                    }
                }

                count -= listCount;
            }

            return count;
        }

        /// <summary>
        /// This resumes any matching coroutines on the current MEC instance if they are currently paused, otherwise it has
        /// no effect.
        /// </summary>
        /// <param name="tag">Any coroutines previously paused with a matching tag will be resumend.</param>
        /// <returns>The number of coroutines that were resumed.</returns>
        public static int ResumeCoroutines(string tag)
        {
            return _instance == null ? 0 : _instance.ResumeCoroutinesOnInstance(tag);
        }

        /// <summary>
        /// This resumes any matching coroutines on this MEC instance if they are currently paused, otherwise it has no effect.
        /// </summary>
        /// <param name="tag">Any coroutines previously paused with a matching tag will be resumend.</param>
        /// <returns>The number of coroutines that were resumed.</returns>
        public int ResumeCoroutinesOnInstance(string tag)
        {
            if (tag == null || !_taggedProcesses.ContainsKey(tag))
                return 0;
            int count = 0;

            var indexesEnum = _taggedProcesses[tag].GetEnumerator();
            while (indexesEnum.MoveNext())
                if (!CoindexIsNull(indexesEnum.Current) && CoindexSetPause(indexesEnum.Current, false))
                    count++;

            var waitingEnum = _waitingTriggers.GetEnumerator();
            while (waitingEnum.MoveNext())
            {
                var pausedList = waitingEnum.Current.Value.GetEnumerator();
                while (pausedList.MoveNext())
                {
                    if (_handleToIndex.ContainsKey(pausedList.Current.Handle) && !CoindexIsNull(_handleToIndex[pausedList.Current.Handle])
                        && !CoindexSetPause(_handleToIndex[pausedList.Current.Handle]))
                        count--;
                }
            }

            return count;
        }

        private void AddTag(string tag, ProcessIndex coindex)
        {
            _processTags.Add(coindex, tag);

            if (_taggedProcesses.ContainsKey(tag))
                _taggedProcesses[tag].Add(coindex);
            else
                _taggedProcesses.Add(tag, new HashSet<ProcessIndex> { coindex });
        }

        private void RemoveTag(ProcessIndex coindex)
        {
            if (_processTags.ContainsKey(coindex))
            {
                if (_taggedProcesses[_processTags[coindex]].Count > 1)
                    _taggedProcesses[_processTags[coindex]].Remove(coindex);
                else
                    _taggedProcesses.Remove(_processTags[coindex]);

                _processTags.Remove(coindex);
            }
        }

        private void MoveTag(ProcessIndex coindexFrom, ProcessIndex coindexTo)
        {
            RemoveTag(coindexTo);

            if (_processTags.ContainsKey(coindexFrom))
            {
                _taggedProcesses[_processTags[coindexFrom]].Remove(coindexFrom);
                _taggedProcesses[_processTags[coindexFrom]].Add(coindexTo);

                _processTags.Add(coindexTo, _processTags[coindexFrom]);
                _processTags.Remove(coindexFrom);
            }
        }

        private bool CoindexKill(ProcessIndex coindex)
        {
            bool retVal;

            switch(coindex.seg)
            {
                case Segment.Update:
                    retVal = UpdateProcesses[coindex.i] != null;
                    UpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.FixedUpdate:
                    retVal = FixedUpdateProcesses[coindex.i] != null;
                    FixedUpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.LateUpdate:
                    retVal = LateUpdateProcesses[coindex.i] != null;
                    LateUpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.SlowUpdate:
                    retVal = SlowUpdateProcesses[coindex.i] != null;
                    SlowUpdateProcesses[coindex.i] = null;
                    return retVal;
            }

            return false;
        }

        private IEnumerator<float> CoindexExtract(ProcessIndex coindex)
        {
            IEnumerator<float> retVal;

            switch (coindex.seg)
            {
                case Segment.Update:
                    retVal = UpdateProcesses[coindex.i];
                    UpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.FixedUpdate:
                    retVal = FixedUpdateProcesses[coindex.i];
                    FixedUpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.LateUpdate:
                    retVal = LateUpdateProcesses[coindex.i];
                    LateUpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.SlowUpdate:
                    retVal = SlowUpdateProcesses[coindex.i];
                    SlowUpdateProcesses[coindex.i] = null;
                    return retVal;
                default:
                    return null;
            }
        }

        private IEnumerator<float> CoindexPeek(ProcessIndex coindex)
        {
            switch (coindex.seg)
            {
                case Segment.Update:
                    return UpdateProcesses[coindex.i];
                case Segment.FixedUpdate:
                    return FixedUpdateProcesses[coindex.i];
                case Segment.LateUpdate:
                    return LateUpdateProcesses[coindex.i];
                case Segment.SlowUpdate:
                    return SlowUpdateProcesses[coindex.i];
                default:
                    return null;
            }
        }

        private bool CoindexIsNull(ProcessIndex coindex)
        {
            switch (coindex.seg)
            {
                case Segment.Update:
                    return UpdateProcesses[coindex.i] == null;
                case Segment.FixedUpdate:
                    return FixedUpdateProcesses[coindex.i] == null;
                case Segment.LateUpdate:
                    return LateUpdateProcesses[coindex.i] == null;
                case Segment.SlowUpdate:
                    return SlowUpdateProcesses[coindex.i] == null;
                default:
                    return true;
            }
        }

        private bool CoindexSetPause(ProcessIndex coindex, bool newPausedState = true)
        {
            bool isPaused;

            switch (coindex.seg)
            {
                case Segment.Update:
                    isPaused = UpdatePaused[coindex.i];
                    UpdatePaused[coindex.i] = newPausedState;
                    return isPaused;
                case Segment.FixedUpdate:
                    isPaused = FixedUpdatePaused[coindex.i];
                    FixedUpdatePaused[coindex.i] = newPausedState;
                    return isPaused;
                case Segment.LateUpdate:
                    isPaused = LateUpdatePaused[coindex.i];
                    LateUpdatePaused[coindex.i] = newPausedState;
                    return isPaused;
                case Segment.SlowUpdate:
                    isPaused = SlowUpdatePaused[coindex.i];
                    SlowUpdatePaused[coindex.i] = newPausedState;
                    return isPaused;
                default:
                    return false;
            }
        }

        private bool CoindexIsPaused(ProcessIndex coindex)
        {
            switch (coindex.seg)
            {
                case Segment.Update:
                    return UpdatePaused[coindex.i];
                case Segment.FixedUpdate:
                    return FixedUpdatePaused[coindex.i];
                case Segment.LateUpdate:
                    return LateUpdatePaused[coindex.i];
                case Segment.SlowUpdate:
                    return SlowUpdatePaused[coindex.i];
                default:
                    return false;
            }
        }

        private IEnumerator<float> CoindexReplace(ProcessIndex coindex, IEnumerator<float> replacement)
        {
            IEnumerator<float> retVal;

            switch (coindex.seg)
            {
                case Segment.Update:
                    retVal = UpdateProcesses[coindex.i];
                    UpdateProcesses[coindex.i] = replacement;
                    return retVal;
                case Segment.FixedUpdate:
                    retVal = FixedUpdateProcesses[coindex.i];
                    FixedUpdateProcesses[coindex.i] = replacement;
                    return retVal;
                case Segment.LateUpdate:
                    retVal = LateUpdateProcesses[coindex.i];
                    LateUpdateProcesses[coindex.i] = replacement;
                    return retVal;
                case Segment.SlowUpdate:
                    retVal = SlowUpdateProcesses[coindex.i];
                    SlowUpdateProcesses[coindex.i] = replacement;
                    return retVal;
                default:
                    return null;
            }
        }

        private static IEnumerator<float> _InjectDelay(IEnumerator<float> proc, double returnAt)
        {
            yield return (float)returnAt;

            ReplacementFunction = delegate { return proc; };
            yield return float.NaN;
        }

        /// <summary>
        /// This will kill all coroutines running on the main MEC instance.
        /// </summary>
        /// <returns>The number of coroutines that were killed.</returns>
        public static void KillAllCoroutines()
        {
            if(_instance != null)
                _instance.KillAllCoroutinesOnInstance();
        }

        /// <summary>
        /// This will kill all coroutines running on the current MEC instance.
        /// </summary>
        /// <returns>The number of coroutines that were killed.</returns>
        public void KillAllCoroutinesOnInstance()
        {
            UpdateProcesses = new IEnumerator<float>[InitialBufferSizeLarge];
            UpdateCoroutines = 0;
            _nextUpdateProcessSlot = 0;

            LateUpdateProcesses = new IEnumerator<float>[InitialBufferSizeSmall];
            LateUpdateCoroutines = 0;
            _nextLateUpdateProcessSlot = 0;

            FixedUpdateProcesses = new IEnumerator<float>[InitialBufferSizeMedium];
            FixedUpdateCoroutines = 0;
            _nextFixedUpdateProcessSlot = 0;

            SlowUpdateProcesses = new IEnumerator<float>[InitialBufferSizeMedium];
            SlowUpdateCoroutines = 0;
            _nextSlowUpdateProcessSlot = 0;

            _processTags.Clear();
            _taggedProcesses.Clear();
            _waitingTriggers.Clear();
            _handleToIndex.Clear();
            _indexToHandle.Clear();
            _expansions = (ushort)((_expansions / 2) + 1);

            ResetTimeCountOnInstance();
        }

        /// <summary>
        /// Kills the instances of the coroutine handle if it exists.
        /// </summary>
        /// <param name="handle">The handle of the coroutine to kill.</param>
        /// <returns>The number of coroutines that were found and killed (0 or 1).</returns>
        public static int KillCoroutines(CoroutineHandle handle)
        {
            return _instance == null ? 0 : _instance.KillCoroutinesOnInstance(handle);
        }

        /// <summary>
        /// Kills the instance of the coroutine handle on this Timing instance if it exists.
        /// </summary>
        /// <param name="handle">The handle of the coroutine to kill.</param>
        /// <returns>The number of coroutines that were found and killed (0 or 1).</returns>
        public int KillCoroutinesOnInstance(CoroutineHandle handle)
        {
            if (handle == null) return 0;
            bool foundOne = false;

            if (_handleToIndex.ContainsKey(handle))
            {
                if (_waitingTriggers.ContainsKey(handle))
                    CloseWaitingProcess(handle);

                foundOne = CoindexExtract(_handleToIndex[handle]) != null;
                RemoveTag(_handleToIndex[handle]);
            }

            return foundOne ? 1 : 0;
        }

        /// <summary>
        /// Kills all coroutines that have the given tag.
        /// </summary>
        /// <param name="tag">All coroutines with this tag will be killed.</param>
        /// <returns>The number of coroutines that were found and killed.</returns>
        public static int KillCoroutines(string tag)
        {
            return _instance == null ? 0 : _instance.KillCoroutinesOnInstance(tag);
        }

        /// <summary> 
        /// Kills all coroutines that have the given tag.
        /// </summary>
        /// <param name="tag">All coroutines with this tag will be killed.</param>
        /// <returns>The number of coroutines that were found and killed.</returns>
        public int KillCoroutinesOnInstance(string tag)
        {
            if (tag == null) return 0;
            int numberFound = 0;

            while (_taggedProcesses.ContainsKey(tag))
            {
                var matchEnum = _taggedProcesses[tag].GetEnumerator();
                matchEnum.MoveNext();

                if (CoindexKill(matchEnum.Current))
                {
                    if (_waitingTriggers.ContainsKey(_indexToHandle[matchEnum.Current]))
                        CloseWaitingProcess(_indexToHandle[matchEnum.Current]);

                    numberFound++;
                }

                RemoveTag(matchEnum.Current);

                if (_indexToHandle.ContainsKey(matchEnum.Current))
                {
                    _handleToIndex.Remove(_indexToHandle[matchEnum.Current]);
                    _indexToHandle.Remove(matchEnum.Current);
                }
            }

            return numberFound;
        }

        /// <summary>
        /// Kills all instances that match both the coroutine handle and the tag on the main Timing instance.
        /// </summary>
        /// <param name="handle">The handle of the coroutine to kill.</param>
        /// <param name="tag">The tag to also match for.</param>
        /// <returns>The number of coroutines that were found and killed (0 or 1).</returns>
        public static int KillCoroutines(CoroutineHandle handle, string tag)
        {
            return _instance == null ? 0 : _instance.KillCoroutinesOnInstance(handle, tag);
        }

        /// <summary>
        /// Kills all instances that match both the coroutine handle and the tag on this Timing instance.
        /// </summary>
        /// <param name="handle">The handle of the coroutine to kill.</param>
        /// <param name="tag">The tag to also match for.</param>
        /// <returns>The number of coroutines that were found and killed (0 or 1).</returns>
        public int KillCoroutinesOnInstance(CoroutineHandle handle, string tag)
        {
            if (handle == null)
                return 0;
            if (tag == null)
                return KillCoroutinesOnInstance(handle);

            bool foundOne = false;
            var waitProcsEnum = _waitingTriggers.GetEnumerator();
            while (waitProcsEnum.MoveNext())
            {
                if (waitProcsEnum.Current.Key == handle && _processTags.ContainsKey(_handleToIndex[handle]) &&
                    _processTags[_handleToIndex[handle]] == tag)
                {
                    CloseWaitingProcess(waitProcsEnum.Current.Key);
                    break;
                }
            }

            if (_handleToIndex.ContainsKey(handle) && _processTags.ContainsKey(_handleToIndex[handle]) && _processTags[_handleToIndex[handle]] == tag)
            {
                foundOne = CoindexExtract(_handleToIndex[handle]) != null;
                RemoveTag(_handleToIndex[handle]);
            }

            return foundOne ? 1 : 0;
        }

        /// <summary>
        /// Use in a yield return statement to wait for the specified number of seconds.
        /// </summary>
        /// <param name="waitTime">Number of seconds to wait.</param>
        public static float WaitForSeconds(float waitTime)
        {
            if (float.IsNaN(waitTime)) waitTime = 0f;
            return LocalTime + waitTime;
        }

        /// <summary>
        /// Use in a yield return statement to wait for the specified number of seconds.
        /// </summary>
        /// <param name="waitTime">Number of seconds to wait.</param>
        public float WaitForSecondsOnInstance(float waitTime)
        {
            if (float.IsNaN(waitTime)) waitTime = 0f;
            return (float)localTime + waitTime;
        }

        /// <summary>
        /// Use the command "yield return Timing.WaitUntilDone(otherCoroutine);" to pause the current 
        /// coroutine until otherCoroutine is done.
        /// </summary>
        /// <param name="otherCoroutine">The coroutine to pause for.</param>
        public static float WaitUntilDone(CoroutineHandle otherCoroutine)
        {
            return Instance.WaitUntilDoneOnInstance(otherCoroutine, true);
        }

        /// <summary>
        /// Use the command "yield return Timing.WaitUntilDone(otherCoroutine);" to pause the current 
        /// coroutine until otherCoroutine is done.
        /// </summary>
        /// <param name="otherCoroutine">The coroutine to pause for.</param>
        /// <param name="warnOnIssue">Post a warning to the console if no hold action was actually performed.</param>
        public static float WaitUntilDone(CoroutineHandle otherCoroutine, bool warnOnIssue)
        {
            return Instance.WaitUntilDoneOnInstance(otherCoroutine, warnOnIssue);
        }

        /// <summary>
        /// Use the command "yield return timingInstance.WaitUntilDoneOnInstance(otherCoroutine);" to pause the current 
        /// coroutine until the otherCoroutine is done.
        /// </summary>
        /// <param name="otherCoroutine">The coroutine to pause for.</param>
        /// <param name="warnOnIssue">Post a warning to the console if no hold action was actually performed.</param>
        public float WaitUntilDoneOnInstance(CoroutineHandle otherCoroutine, bool warnOnIssue)
        {
            if (otherCoroutine == null)
            {
                if (warnOnIssue)
                    Debug.LogWarning("A null handle was passed into WaitUntilDone.");

                return 0f;
            }

            if (_handleToIndex.ContainsKey(otherCoroutine))
            {
                if (CoindexIsNull(_handleToIndex[otherCoroutine]))
                    return 0f;

                if (!_waitingTriggers.ContainsKey(otherCoroutine))
                {
                    CoindexReplace(_handleToIndex[otherCoroutine], _StartWhenDone(otherCoroutine, CoindexPeek(_handleToIndex[otherCoroutine])));
                    _waitingTriggers.Add(otherCoroutine, new HashSet<ProcessData>());
                }

                ReplacementFunction = (coptr, instance, handle) =>
                {
                    if (handle == otherCoroutine)
                    {
                        if (warnOnIssue)
                            Debug.LogWarning("A coroutine attempted to wait for itself.");

                        return coptr;
                    }

                    _waitingTriggers[otherCoroutine].Add(new ProcessData
                    {
                        Handle = handle,
                        PauseTime = coptr.Current > GetSegmentTime(_handleToIndex[handle].seg)
                            ? coptr.Current - (float)GetSegmentTime(_handleToIndex[handle].seg) : 0f
                    });

                    CoindexSetPause(_handleToIndex[handle]);

                    return coptr;
                };

                return float.NaN;
            }

            if (warnOnIssue)
                Debug.LogWarning("WaitUntilDone cannot hold: The coroutine handle that was passed in is invalid.\n" + otherCoroutine);

            return 0f;
        }

        private IEnumerator<float> _StartWhenDone(CoroutineHandle handle, IEnumerator<float> proc)
        {
            if (!_waitingTriggers.ContainsKey(handle)) yield break;

            try
            {
                if (proc.Current > localTime)
                    yield return proc.Current;

                while (proc.MoveNext())
                    yield return proc.Current;
            }
            finally
            {
                CloseWaitingProcess(handle);
            }
        }

        private void CloseWaitingProcess(CoroutineHandle handle)
        {
            if (!_waitingTriggers.ContainsKey(handle)) return;

            var tasksEnum = _waitingTriggers[handle].GetEnumerator();
            _waitingTriggers.Remove(handle);

            while (tasksEnum.MoveNext())
            {
                if (_handleToIndex.ContainsKey(tasksEnum.Current.Handle))
                {
                    ProcessIndex coIndex = _handleToIndex[tasksEnum.Current.Handle];

                    if (tasksEnum.Current.PauseTime > 0d)
                        CoindexReplace(coIndex, _InjectDelay(CoindexPeek(coIndex), (float)(GetSegmentTime(coIndex.seg) + tasksEnum.Current.PauseTime)));

                    CoindexSetPause(coIndex, false);
                }
            }
        }

        /// <summary>
        /// Use the command "yield return Timing.WaitUntilDone(wwwObject);" to pause the current 
        /// coroutine until the wwwObject is done.
        /// </summary>
        /// <param name="wwwObject">The www object to pause for.</param>
        public static float WaitUntilDone(WWW wwwObject)
        {
            if (wwwObject == null || wwwObject.isDone) return 0f;
            ReplacementFunction = (input, timing, tag) => _StartWhenDone(wwwObject, input);
            return float.NaN;
        }

        private static IEnumerator<float> _StartWhenDone(WWW www, IEnumerator<float> pausedProc)
        {
            while (!www.isDone)
                yield return 0f;

            ReplacementFunction = delegate { return pausedProc; };
            yield return float.NaN;
        }

        /// <summary>
        /// Use the command "yield return Timing.WaitUntilDone(operation);" to pause the current 
        /// coroutine until the operation is done.
        /// </summary>
        /// <param name="operation">The operation variable returned.</param>
        public static float WaitUntilDone(AsyncOperation operation)
        {
            if (operation == null || operation.isDone) return 0f;
            ReplacementFunction = (input, timing, tag) => _StartWhenDone(operation, input);
            return float.NaN;
        }

        private static IEnumerator<float> _StartWhenDone(AsyncOperation operation, IEnumerator<float> pausedProc)
        {
            while (!operation.isDone)
                yield return 0f;

            ReplacementFunction = delegate { return pausedProc; };
            yield return float.NaN;
        }

#if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
        /// <summary>
        /// Use the command "yield return Timing.WaitUntilDone(operation);" to pause the current 
        /// coroutine until the operation is done.
        /// </summary>
        /// <param name="operation">The operation variable returned.</param>
        public static float WaitUntilDone(CustomYieldInstruction operation)
        {
            if (operation == null || !operation.keepWaiting) return 0f;
            ReplacementFunction = (input, timing, tag) => _StartWhenDone(operation, input);
            return float.NaN;
        }

        private static IEnumerator<float> _StartWhenDone(CustomYieldInstruction operation, IEnumerator<float> pausedProc)
        {
            while (operation.keepWaiting)
                yield return 0f;

            ReplacementFunction = delegate { return pausedProc; };
            yield return float.NaN;
        }
#endif

        /// <summary>
        /// Calls the specified action after a specified number of seconds.
        /// </summary>
        /// <param name="delay">The number of seconds to wait before calling the action.</param>
        /// <param name="action">The action to call.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallDelayed(float delay, System.Action action)
        {
            return action == null ? null : RunCoroutine(Instance._DelayedCall(delay, action));
        }

        /// <summary>
        /// Calls the specified action after a specified number of seconds.
        /// </summary>
        /// <param name="delay">The number of seconds to wait before calling the action.</param>
        /// <param name="action">The action to call.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallDelayedOnInstance(float delay, System.Action action)
        {
            return action == null ? null : RunCoroutineOnInstance(_DelayedCall(delay, action));
        }

        private IEnumerator<float> _DelayedCall(float delay, System.Action action)
        {
            yield return WaitForSecondsOnInstance(delay);

            action();
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallPeriodically(float timeframe, float period, System.Action action, System.Action onDone = null)
        {
            return action == null ? null : RunCoroutine(Instance._CallContinuously(timeframe, period, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallPeriodicallyOnInstance(float timeframe, float period, System.Action action, System.Action onDone = null)
        {
            return action == null ? null : RunCoroutineOnInstance(_CallContinuously(timeframe, period, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallPeriodically(float timeframe, float period, System.Action action, Segment timing, System.Action onDone = null)
        {
            return action == null ? null : RunCoroutine(Instance._CallContinuously(timeframe, period, action, onDone), timing);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallPeriodicallyOnInstance(float timeframe, float period, System.Action action, Segment timing, System.Action onDone = null)
        {
            return action == null ? null : RunCoroutineOnInstance(_CallContinuously(timeframe, period, action, onDone), timing);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallContinuously(float timeframe, System.Action action, System.Action onDone = null)
        {
            return action == null ? null : RunCoroutine(Instance._CallContinuously(timeframe, 0f, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallContinuouslyOnInstance(float timeframe, System.Action action, System.Action onDone = null)
        {
            return action == null ? null : RunCoroutineOnInstance(_CallContinuously(timeframe, 0f, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallContinuously(float timeframe, System.Action action, Segment timing, System.Action onDone = null)
        {
            return action == null ? null : RunCoroutine(Instance._CallContinuously(timeframe, 0f, action, onDone), timing);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallContinuouslyOnInstance(float timeframe, System.Action action, Segment timing, System.Action onDone = null)
        {
            return action == null ? null : RunCoroutineOnInstance(_CallContinuously(timeframe, 0f, action, onDone), timing);
        }

        private IEnumerator<float> _CallContinuously(float timeframe, float period, System.Action action, System.Action onDone)
        {
            double startTime = localTime;
            while (localTime <= startTime + timeframe)
            {
                yield return WaitForSecondsOnInstance(period);

                action();
            }

            if (onDone != null)
                onDone();
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each period.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallPeriodically<T>(T reference, float timeframe, float period, System.Action<T> action, System.Action<T> onDone = null)
        {
            return action == null ? null : RunCoroutine(Instance._CallContinuously(reference, timeframe, period, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each period.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallPeriodicallyOnInstance<T>(T reference, float timeframe, float period, System.Action<T> action, System.Action<T> onDone = null)
        {
            return action == null ? null : RunCoroutineOnInstance(_CallContinuously(reference, timeframe, period, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each period.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallPeriodically<T>(T reference, float timeframe, float period, System.Action<T> action, 
            Segment timing, System.Action<T> onDone = null)
        {
            return action == null ? null : RunCoroutine(Instance._CallContinuously(reference, timeframe, period, action, onDone), timing);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each period.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallPeriodicallyOnInstance<T>(T reference, float timeframe, float period, System.Action<T> action,
            Segment timing, System.Action<T> onDone = null)
        {
            return action == null ? null : RunCoroutineOnInstance(_CallContinuously(reference, timeframe, period, action, onDone), timing);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each frame.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallContinuously<T>(T reference, float timeframe, System.Action<T> action, System.Action<T> onDone = null)
        {
            return action == null ? null : RunCoroutine(Instance._CallContinuously(reference, timeframe, 0f, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each frame.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallContinuouslyOnInstance<T>(T reference, float timeframe, System.Action<T> action, System.Action<T> onDone = null)
        {
            return action == null ? null : RunCoroutineOnInstance(_CallContinuously(reference, timeframe, 0f, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each frame.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public static CoroutineHandle CallContinuously<T>(T reference, float timeframe, System.Action<T> action, 
            Segment timing, System.Action<T> onDone = null)
        {
            return action == null ? null : RunCoroutine(Instance._CallContinuously(reference, timeframe, 0f, action, onDone), timing);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each frame.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        /// <returns>The handle to the coroutine that is started by this function.</returns>
        public CoroutineHandle CallContinuouslyOnInstance<T>(T reference, float timeframe, System.Action<T> action,
            Segment timing, System.Action<T> onDone = null)
        {
            return action == null ? null : RunCoroutineOnInstance(_CallContinuously(reference, timeframe, 0f, action, onDone), timing);
        }

        private IEnumerator<float> _CallContinuously<T>(T reference, float timeframe, float period,
            System.Action<T> action, System.Action<T> onDone = null)
        {
            double startTime = localTime;
            while (localTime <= startTime + timeframe)
            {
                yield return WaitForSecondsOnInstance(period);

                action(reference);
            }

            if (onDone != null)
                onDone(reference);
        }

        private struct ProcessData : System.IEquatable<ProcessData>
        {
            public CoroutineHandle Handle;
            public float PauseTime;

            public bool Equals(ProcessData other)
            {
                return Handle == other.Handle;
            }

            public override bool Equals(object other)
            {
                if (other is ProcessData)
                    return Equals((ProcessData)other);
                return false;
            }

            public override int GetHashCode()
            {
                return Handle.GetHashCode();
            }
        }

        private struct ProcessIndex : System.IEquatable<ProcessIndex>
        {
            public Segment seg;
            public int i;

            public bool Equals(ProcessIndex other)
            {
                return seg == other.seg && i == other.i;
            }

            public override bool Equals(object other)
            {
                if (other is ProcessIndex)
                    return Equals((ProcessIndex)other);
                return false;
            }

            public static bool operator ==(ProcessIndex a, ProcessIndex b)
            {
                return a.seg == b.seg && a.i == b.i;
            }

            public static bool operator !=(ProcessIndex a, ProcessIndex b)
            {
                return a.seg != b.seg || a.i != b.i;
            }

            public override int GetHashCode()
            {
                return (((int)seg - 2) * (int.MaxValue / 3)) + i;
            }
        }

        [System.Obsolete("Unity coroutine function, use RunCoroutine instead.", true)]
        public new Coroutine StartCoroutine(System.Collections.IEnumerator routine) { return null; }

        [System.Obsolete("Unity coroutine function, use RunCoroutine instead.", true)]
        public new Coroutine StartCoroutine(string methodName, object value) { return null; }

        [System.Obsolete("Unity coroutine function, use RunCoroutine instead.", true)]
        public new Coroutine StartCoroutine(string methodName) { return null; }

        [System.Obsolete("Unity coroutine function, use RunCoroutine instead.", true)]
        public new Coroutine StartCoroutine_Auto(System.Collections.IEnumerator routine) { return null; }

        [System.Obsolete("Unity coroutine function, use KillCoroutine instead.", true)]
        public new void StopCoroutine(string methodName) { }

        [System.Obsolete("Unity coroutine function, use KillCoroutine instead.", true)]
        public new void StopCoroutine(System.Collections.IEnumerator routine) { }

        [System.Obsolete("Unity coroutine function, use KillCoroutine instead.", true)]
        public new void StopCoroutine(Coroutine routine) { }

        [System.Obsolete("Unity coroutine function, use KillAllCoroutines instead.", true)]
        public new void StopAllCoroutines() { }

        [System.Obsolete("Use your own GameObject for this.", true)]
        public new static void Destroy(Object obj) { }

        [System.Obsolete("Use your own GameObject for this.", true)]
        public new static void Destroy(Object obj, float f) { }

        [System.Obsolete("Use your own GameObject for this.", true)]
        public new static void DestroyObject(Object obj) { }

        [System.Obsolete("Use your own GameObject for this.", true)]
        public new static void DestroyObject(Object obj, float f) { }

        [System.Obsolete("Use your own GameObject for this.", true)]
        public new static void DestroyImmediate(Object obj) { }

        [System.Obsolete("Use your own GameObject for this.", true)]
        public new static void DestroyImmediate(Object obj, bool b) { }

        [System.Obsolete("Just.. no.", true)]
        public new static T FindObjectOfType<T>() where T : Object { return null; }

        [System.Obsolete("Just.. no.", true)]
        public new static Object FindObjectOfType(System.Type t) { return null; }

        [System.Obsolete("Just.. no.", true)]
        public new static T[] FindObjectsOfType<T>() where T : Object { return null; }

        [System.Obsolete("Just.. no.", true)]
        public new static Object[] FindObjectsOfType(System.Type t) { return null; }

        [System.Obsolete("Just.. no.", true)]
        public new static void print(object message) { }
    }

    public enum Segment
    {
        Update,
        FixedUpdate,
        LateUpdate,
        SlowUpdate,
    }

    public sealed class CoroutineHandle { }
}

public static class MECExtensionMethods
{
    /// <summary>
    /// Cancels this coroutine when the supplied game object is destroyed or made inactive.
    /// </summary>
    /// <param name="coroutine">The coroutine handle to act upon.</param>
    /// <param name="gameObject">The GameObject to test.</param>
    /// <returns>The modified coroutine handle.</returns>
    public static IEnumerator<float> CancelWith(this IEnumerator<float> coroutine, GameObject gameObject)
    {
        while (gameObject && gameObject.activeInHierarchy && coroutine.MoveNext())
            yield return coroutine.Current;
    }

    /// <summary>
    /// Cancels this coroutine when the supplied game objects are destroyed or made inactive.
    /// </summary>
    /// <param name="coroutine">The coroutine handle to act upon.</param>
    /// <param name="gameObject1">The first GameObject to test.</param>
    /// <param name="gameObject2">The second GameObject to test</param>
    /// <returns>The modified coroutine handle.</returns>
    public static IEnumerator<float> CancelWith(this IEnumerator<float> coroutine, GameObject gameObject1, GameObject gameObject2)
    {
        while (gameObject1 && gameObject1.activeInHierarchy && gameObject2 && gameObject2.activeInHierarchy && coroutine.MoveNext())
            yield return coroutine.Current;
    }

    /// <summary>
    /// Cancels this coroutine when the supplied game objects are destroyed or made inactive.
    /// </summary>
    /// <param name="coroutine">The coroutine handle to act upon.</param>
    /// <param name="gameObject1">The first GameObject to test.</param>
    /// <param name="gameObject2">The second GameObject to test</param>
    /// <param name="gameObject3">The third GameObject to test.</param>
    /// <returns>The modified coroutine handle.</returns>
    public static IEnumerator<float> CancelWith(this IEnumerator<float> coroutine,
        GameObject gameObject1, GameObject gameObject2, GameObject gameObject3)
    {
        while (gameObject1 && gameObject1.activeInHierarchy && gameObject2 && gameObject2.activeInHierarchy &&
                gameObject3 && gameObject3.activeInHierarchy && coroutine.MoveNext())
            yield return coroutine.Current;
    }
}
