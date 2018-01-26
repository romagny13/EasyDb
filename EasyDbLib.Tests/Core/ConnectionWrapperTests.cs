using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Threading.Tasks;

namespace EasyDbLib.Tests.Core
{
    [TestClass]
    public class ConnectionWrapperTests
    {

        [TestMethod]
        public async Task OpenClose_WithConnectionStrategyAuto()
        {
            var service = new ConnectionWrapper(DbConstants.SqlDb1, "System.Data.SqlClient", ConnectionStrategy.Auto);

            await service.CheckStrategyAndOpenAsync();

            Assert.AreEqual(ConnectionState.Open, service.State);

            service.CheckStrategyAndClose();

            Assert.AreEqual(ConnectionState.Closed, service.State);
        }

        [TestMethod]
        public async Task OpenClose_WithConnectionStrategyManual()
        {
            var service = new ConnectionWrapper(DbConstants.SqlDb1, "System.Data.SqlClient", ConnectionStrategy.Manual);

            await service.CheckStrategyAndOpenAsync();

            Assert.AreEqual(ConnectionState.Closed, service.State);

            await service.OpenAsync();

            Assert.AreEqual(ConnectionState.Open, service.State);

            service.CheckStrategyAndClose();

            Assert.AreEqual(ConnectionState.Open, service.State);

            service.Close();

            Assert.AreEqual(ConnectionState.Closed, service.State);
        }
    }

}
