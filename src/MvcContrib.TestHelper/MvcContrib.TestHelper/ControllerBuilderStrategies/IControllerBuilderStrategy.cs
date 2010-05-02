namespace MvcContrib.TestHelper.ControllerBuilderStrategies
{
	public interface IControllerBuilderStrategy 
	{
		void Setup(TestControllerBuilder testControllerBuilder);
	}
}