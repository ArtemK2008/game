using NUnit.Framework;
using Survivalon;

namespace Survivalon.Tests.EditMode
{
    public sealed class RuntimeAssemblySmokeTests
    {
        [Test]
        public void ShouldReferenceRuntimeAssembly()
        {
            Assert.That(typeof(RuntimeAssemblyMarker).Assembly.GetName().Name, Is.EqualTo("Survivalon.Runtime"));
        }
    }
}

