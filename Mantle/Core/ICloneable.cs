namespace Mantle
{
    public interface ICloneable<T>
    {
        T ShallowCopy();

        T DeepCopy();
    }
}