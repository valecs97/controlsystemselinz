using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control_system
{
    class exception
    {
        encryption enc;
        public exception()
        {
            enc = new encryption();
        }

        /*
        Function that checks if the received data can be decrypted according to the common password established by our subsystems
        */

        public string checkData(string data)
        {
            data = enc.Decrypt(data, "Some random password");
            if (data == "FailedToDecrypt")
                return null;
            return data;
        }

        /*
        Function not yet implemented
        It should only create a error code and send to the other subsystems
        */

        public string systemFailure()
        {
            return null;
        }
    }
}
