namespace App.Mapping
{
    public interface IMapper
    {
        string ToString(object obj);
        
        string ToPrettyString(object obj, int spaceCount);
    }
}