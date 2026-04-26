using System.Threading.Tasks;
using JsonModel;

public interface IIngameView : IView
{
    public Task Initialize(StageModel stageModel);
}
