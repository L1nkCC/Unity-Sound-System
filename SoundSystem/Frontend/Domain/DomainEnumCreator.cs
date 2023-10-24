using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CC.SoundSystem
{
    public static class DomainEnumWriter
    {
        private static string GetEnumFileName(this string domainName) { return domainName + "Type"; }
        public static void CreateDomainEnum(string domainName)
        {
            CC.Enum.EnumWriter.WriteEnumFile(domainName.GetEnumFileName(), Domain.GetNodeNames(domainName), "CC.SoundSystem");
        }
        public static void DeleteDomainEnum(string domainName)
        {
            CC.Enum.EnumWriter.DeleteEnumFile(domainName.GetEnumFileName());
        }
    }
}
