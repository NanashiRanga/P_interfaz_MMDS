using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Registro_FGO_MMDS
{
    internal class Conexion
    {
        public static MySqlConnection conexion() {
            String servidor = "localhost";
            String bd = "tiendaproyecto";
            String usuario = "root";
            String pass = "Pekora2002@";
            String cadenaconexion = "Database="+bd+"; Server="+servidor+";User ID="+usuario+";Password="+pass+"";

            try {
            MySqlConnection BDconexion = new MySqlConnection(cadenaconexion);
                return BDconexion;

            }catch(MySqlException ex) {
            Console.WriteLine("Error: "+ex.Message);
                return null;
            }



        }
    }
}
