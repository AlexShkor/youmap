namespace YouMap.Framework.Mvc.Validation
{
    public interface IValidationRepository
    {
        bool Validates<T>(T input);
    }
}