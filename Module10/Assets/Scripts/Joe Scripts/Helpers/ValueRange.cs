[System.Serializable]
public struct ValueRange<T>
{
    public ValueRange(T min, T max)
    {
        Min = min;
        Max = max;
    }

    public T Min;
    public T Max;
}