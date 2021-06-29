namespace SIMPLEAPI_Demo.Impresion.Core.Classes
{
    public abstract class PrinterColumn
    {
        public enum Aligns
        {
            Center,
            Right,
            Left
        }
        public Aligns Align { get; set; }

        public int MaxChars { get; set; }
    }
}
