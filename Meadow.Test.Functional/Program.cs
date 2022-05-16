using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json.Serialization;

namespace Meadow.Test.Functional
{
    class Program
    {
      

        static void Main(string[] args)
        {
            new RetrieveDataFromDatabaseTest().Main();
        }

    }
}