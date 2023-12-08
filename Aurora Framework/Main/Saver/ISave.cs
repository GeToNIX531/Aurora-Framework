namespace Data.Saver
{
    public interface ISave
    {
        string GetSavePath();
        object GetObject();
    }
}
