using System.Threading.Tasks;
using UnityEngine;

public class coucou : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            _ = OnLapCompleted();
        }
    }
    
    public async Task OnLapCompleted()
    {
        await DataManager.Instance.IncrementCell("F3");
    }

}
