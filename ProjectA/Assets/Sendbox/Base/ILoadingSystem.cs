using System.Threading.Tasks;

public interface ILoadingSystem : ISystem
{
    public void SetValue(float value);
    public void Show();
    public void Hide();

}