using System;
using EFDDD.DataModel.Migrations.AzureImportExport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataModel.Migrations.Test
{
    [TestClass]
    public class AzureImportExport_ExtensionsTest
    {
        [TestClass]
        public class ToParsedAzureStorageConnectionMethod : AzureImportExport_ExtensionsTest
        {
            [TestClass]
            public class AccountKeyProperty : ToParsedAzureStorageConnectionMethod
            {
                [TestMethod]
                public void ShouldEqual()
                {
                    //Arrange
                    var azureconn = Constants.AZURE_STORAGE_CONNECTIONSTRING;
                    var azureconnparsed = azureconn.ToParsedAzureStorageConnection();

                    //Act
                    var actual = azureconnparsed["AccountKey"];

                    //Assert
                    Assert.AreEqual("******", actual);
                }
            }

            [TestClass]
            public class AccountNameProperty : ToParsedAzureStorageConnectionMethod
            {
                [TestMethod]
                public void ShouldEqual()
                {
                    //Arrange
                    var azureconn = Constants.AZURE_STORAGE_CONNECTIONSTRING;
                    var azureconnparsed = azureconn.ToParsedAzureStorageConnection();

                    //Act
                    var actual = azureconnparsed["AccountName"];

                    //Assert
                    Assert.AreEqual("******", actual);
                }
            }

            [TestClass]
            public class ProtocolProperty : ToParsedAzureStorageConnectionMethod
            {
                [TestMethod]
                public void ShouldEqual()
                {
                    //Arrange
                    var azureconn = Constants.AZURE_STORAGE_CONNECTIONSTRING;
                    var azureconnparsed = azureconn.ToParsedAzureStorageConnection();

                    //Act
                    var actual = azureconnparsed["Protocol"];

                    //Assert
                    Assert.AreEqual("https", actual);
                }
            }
        }
    }
}
