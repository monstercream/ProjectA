public class LoadingSystem : ILoadingSystem
{
    public void SetValue(float value)
    {
        LoadingView.View.SetValue(value);
    }

    public void Show()
    {
        LoadingView.View.Show();
    }

    public void Hide()
    {
        LoadingView.View.Hide();
    }

    public void Dispose()
    {
    }
}