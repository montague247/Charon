using Charon.Dojo;

[assembly: AssemblySecurityDojo("SecureEncryptTests", "Security", ConfigurationPath = "SecureEncryptTests.json", Stages = ["TESTS"], Priority = int.MaxValue - 1)]
