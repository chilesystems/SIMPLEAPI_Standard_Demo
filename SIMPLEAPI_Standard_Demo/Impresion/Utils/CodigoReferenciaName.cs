﻿using System.Collections.Generic;

namespace SIMPLEAPI_Demo.Impresion.Utils
{
    public class CodigoReferenciaName
    {
        public static Dictionary<int, string> Names =
            new Dictionary<int, string>()
            {
                {1, "Anula documento de referencia" },
                {2, "Corrige texto de documento de referencia" },
                {3, "Corrige montos de documento de referencia" },
            };
    }
}
