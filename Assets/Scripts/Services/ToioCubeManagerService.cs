using toio;

public class ToioCubeManagerService
{
    private static ToioCubeManagerService instance;

    public CubeManager CubeManager { get; private set; }

    public static ToioCubeManagerService Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ToioCubeManagerService();
                instance.CubeManager = new CubeManager();
            }

            return instance;
        }
    }
}
