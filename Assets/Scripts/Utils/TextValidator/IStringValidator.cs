namespace FlavorfulStory
{
    public interface IStringValidator
    {
        bool IsValid(string input, out string error);
    }
}