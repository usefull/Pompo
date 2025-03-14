namespace WasmModule
{
    public class UtilityService
    {
        private readonly DateTime _creation;

        public UtilityService()
        {
            _creation = DateTime.Now;
            Console.WriteLine("Utility service has created");
        }

        public DateTime GetCreationTime() => _creation;
    }
}