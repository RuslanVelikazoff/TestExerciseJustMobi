using System;
using System.Collections.Generic;

public interface IReadOnlyCubeCollection : IEnumerable<Cube>
{
    public int Count { get; }
    public Cube FirstCube { get; }
    public Cube LastCube { get; }
    public bool HasCubes { get; }
    public event Action<Cube> CubeAdded;
    public event Action<RemovedCubeData> CubeRemoved;
    public event Action<int> CountChanged;
    public Cube this[int index] { get; }
}