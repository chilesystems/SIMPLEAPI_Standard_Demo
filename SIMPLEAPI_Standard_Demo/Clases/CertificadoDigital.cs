namespace SIMPLEAPI_Demo.Clases
{
    public class CertificadoDigital
    {
        public string Nombre { get; set; }
        public string Rut { get; set; }

        public int RutCuerpo { get { return int.Parse(Rut.Substring(0, Rut.Length - 2)); } }
        public string DV { get { return Rut.Substring(Rut.Length - 1, 1); } }
    }
}
