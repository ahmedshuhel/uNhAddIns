For the first time.
Because we dont have AssemblyInfo.cs files into repository, u should run 'generateassemblyinfo.bat' for generate these files before open any solution in Visual Studio.


[HowTo run test]

Before you run any test you must:
1-Create a Database named uNhAITests
2-Copy each hibernate.cfg.template from uNhAddIns.XYZAdapters.Tests to his respective project folder, rename the file to hibernate.cfg.xml and change the connection settings to fit your needs. 
4-Copy one template from "uNhAddIns\ConfigurationTemplates" to unhaddins.Test project folder rename this file to hibernate.cfg.xml and change the connection settings to fit your needs. 