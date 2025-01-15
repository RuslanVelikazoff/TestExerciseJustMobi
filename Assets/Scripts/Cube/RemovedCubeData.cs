public struct RemovedCubeData
{
    public RemovedCubeData(int index, Cube removedCube, Cube newCube, Cube previousCube)
    {
        Index = index;
        RemovedCube = removedCube;
        NewCube = newCube;
        PreviousCube = previousCube;
    }

    public int Index { get; }
    public Cube RemovedCube { get; }
    public Cube NewCube { get; }
    public Cube PreviousCube { get; }
}