using System.Configuration;

namespace EasyDbLib
{
    public class ConfigurationProtectionManager
    {
       const string connectionStringsSection = "connectionStrings";

        private static bool TryEncrypt(Configuration configuration, string sectionName)
        {
            var section = configuration.GetSection(sectionName);
            if (section != null)
            {
                if (!section.SectionInformation.IsProtected)
                {
                    section.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                    configuration.Save(ConfigurationSaveMode.Full, true);
                    return true;
                }
            }
            return false;
        }

        public static bool EncryptSection(string exePath, string sectionName)
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(exePath);
            return TryEncrypt(configuration, sectionName);
        }

        public static bool EncryptSection(string sectionName)
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return TryEncrypt(configuration, sectionName);
        }

        public static bool EncryptConnectionStringsSection()
        {
            return EncryptSection(connectionStringsSection);
        }

        public static bool EncryptConnectionStringsSection(string exePath)
        {
            return EncryptSection(exePath, connectionStringsSection);
        }
    }
}
