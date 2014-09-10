using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace WallRiskEngine
{

    public class Class1
    {
        const string pathToDll = getDllPathFollowingSystem();

        private static String getDllPathFollowingSystem()
        {
            if (System.Environment.Is64BitOperatingSystem)
            {
                return @"lib\x64\wre-ensimag-c-4.1.dll";
            } else
	        {
                return @"lib\x86\wre-ensimag-c-4.1.dll";
	        }
        }

    }
}
