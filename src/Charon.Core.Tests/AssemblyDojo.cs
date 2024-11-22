using Charon.Dojo;

[assembly: AssemblySecurityDojo("SecureEncryptTest", "Security", ConfigurationPath = "SecureEncryptTest.json", Stages = ["TESTS"], Priority = int.MaxValue - 1)]
