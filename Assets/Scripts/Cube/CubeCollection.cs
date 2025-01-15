using System.Collections.Generic;
using System;
using System.Collections;

public class CubeCollection : IReadOnlyCubeCollection
{
    private List<Cube> _Cubes = new List<Cube>();
    public int Count => _Cubes.Count;
    public Cube FirstCube => HasCubes ? _Cubes[0] : default;
    public Cube LastCube => HasCubes ? _Cubes[Count - 1] : default;
    public bool HasCubes => _Cubes.Count != 0;
    public event Action<Cube> CubeAdded;
    public event Action<RemovedCubeData> CubeRemoved;
    public event Action<int> CountChanged;
    private int _NoResultsIndex = -1;

    public Cube this[int index] => _Cubes[index];

    public IEnumerator<Cube> GetEnumerator() => _Cubes.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => _Cubes.GetEnumerator();

    public void Add(Cube cube)
    {
        _Cubes.Add(cube);
        CubeAdded?.Invoke(cube);
        CountChanged?.Invoke(Count);
    }

    public bool Remove(Cube cube) 
    { 
        int index = _Cubes.IndexOf(cube);

        if (index == _NoResultsIndex)
            return false;

        RemoveAt(index);
        return true;
    }

    public void RemoveAt(int index)
    {
        if (index >= _Cubes.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        Cube cube = _Cubes[index];
        _Cubes.RemoveAt(index);

        Cube newCube = index < Count ? _Cubes[index] : null;
        Cube previousCube = index - 1 >= 0 ? _Cubes[index - 1] : null;

        RemovedCubeData data = new RemovedCubeData(index, cube, newCube, previousCube);

        CubeRemoved?.Invoke(data);
        CountChanged?.Invoke(Count);
    }
}
