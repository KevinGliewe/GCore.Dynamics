namespace GCore.Dynamics.Json.Tests.JsonDynamicTests;

public abstract class BaseTest
{
    private dynamic json = Constants.LoadTestModelSimple();
    public dynamic Json { get { return json; } }
}